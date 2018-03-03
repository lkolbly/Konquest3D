using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class Planet : NetworkBehaviour
{
    [SyncVar(hook = "OnSetTeamId")]
    public int teamId;

    public GameObject playerObject;

    public GameObject shipPrefab;
    public GameObject linePrefab;

    public float shipSpeed;

    public GameObject selectedHalo;
    public GameObject redTeamHalo;
    public GameObject blueTeamHalo;
    public GameObject tooltip;

    public float constructionTime;
    public float shipEffectiveness;
    private float constructionCooldown;

    [SyncVar(hook = "OnChangeNumberOfShips")]
    private int numberOfShips;

    private bool isBeingTouched = false;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting planet!");
        DisplayTeamColor();

        GetComponent<VRTK_InteractableObject>().InteractableObjectTouched += new InteractableObjectEventHandler(StartTouching);
        GetComponent<VRTK_InteractableObject>().InteractableObjectUntouched += new InteractableObjectEventHandler(StopTouching);

        constructionCooldown = constructionTime;

        UpdateTooltip();
    }

    public void DisplayTeamColor()
    {
        ((Behaviour)selectedHalo.GetComponent("Halo")).enabled = false;
        ((Behaviour)redTeamHalo.GetComponent("Halo")).enabled = false;
        ((Behaviour)blueTeamHalo.GetComponent("Halo")).enabled = false;

        var networkManager = GameObject.Find("NetworkLobby");
        var localPlayerObject = networkManager == null ?
            playerObject :
            networkManager.GetComponent<MyNetworkManager>().localPlayerObject;
        //Debug.Log(networkManager+" "+localPlayerObject);
        if (localPlayerObject == null) return;

        var player = localPlayerObject.GetComponent<Player>();
        if (player.teamId == teamId)
        {
            ((Behaviour)blueTeamHalo.GetComponent("Halo")).enabled = true;
        }
        else if (teamId != 0)
        {
            ((Behaviour)redTeamHalo.GetComponent("Halo")).enabled = true;
        }
    }

    public void DisplaySelected()
    {
        ((Behaviour)selectedHalo.GetComponent("Halo")).enabled = true;
        ((Behaviour)redTeamHalo.GetComponent("Halo")).enabled = false;
        ((Behaviour)blueTeamHalo.GetComponent("Halo")).enabled = false;
    }

    public void UpdateTooltip()
    {
        var tooltipScript = tooltip.GetComponent<VRTK_ObjectTooltip>();
        tooltipScript.UpdateText(numberOfShips+" ships");
    }

    void OnChangeNumberOfShips(int nships)
    {
        numberOfShips = nships;
        UpdateTooltip();
    }

    void OnSetTeamId(int newTeamId)
    {
        teamId = newTeamId;
        UpdateTooltip();
        DisplayTeamColor();
    }

    private string PadNumber(int n, int length)
    {
        var s = n.ToString("D");
        return new string('0', length - s.Length) + s;
    }

    [ClientRpc]
    public void RpcSetGraphics(int graphicId)
    {
        Debug.Log("Setting graphics to " + graphicId);
        var prefab = Resources.Load("generated/planet" + PadNumber(graphicId, 3), typeof(GameObject)) as GameObject;
        var planetGraphics = Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
        planetGraphics.transform.parent = gameObject.transform;
        planetGraphics.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    [Command]
    public void CmdLaunchFleet(GameObject target, int numShips)
    {
        LaunchFleet(target, numShips);
    }

    public void LaunchFleetDispatch(GameObject target, int numShips)
    {
        if (isClient)
        {
            CmdLaunchFleet(target, numShips);
        }
        else
        {
            LaunchFleet(target, numShips);
        }
    }

    private void LaunchFleet(GameObject target, int numShips)
    {
        numberOfShips -= numShips;

        var direction = Vector3.Normalize(target.transform.position - transform.position);
        var shipObj = Instantiate(shipPrefab, gameObject.transform.position, Quaternion.FromToRotation(new Vector3(0,0,1), direction));
        var fleet = shipObj.GetComponent<Ship>();
        fleet.SetStats(teamId, numShips, shipEffectiveness);

        // Send it to the target
        shipObj.GetComponent<Rigidbody>().velocity = shipSpeed * direction;
        shipObj.GetComponent<Ship>().targetPlanet = target;

        // Draw a line going there
        var lineObject = Instantiate(linePrefab);
        var line = lineObject.GetComponent<LineController>();
        line.startPosition = transform.position;
        line.endPosition = target.transform.position;
        shipObj.GetComponent<Ship>().line = lineObject;

        if (isServer) // @TODO: Should this be in CmdLaunchFleet?
        {
            // Go get the player object of the owner
            GameObject player = null;
            foreach (var go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.GetComponent<Player>() != null && go.GetComponent<Player>().teamId == teamId)
                {
                    player = go;
                    break;
                }
            }
            
            // Update the network about the new ship
            NetworkServer.SpawnWithClientAuthority(shipObj, player);
            NetworkServer.SpawnWithClientAuthority(lineObject, player);
        }
    }

    public int GetNumberOfShips()
    {
        return numberOfShips;
    }

    public void DoInvasion(int attackingTeamId, int attackingNumShips, float attackingEffectiveness)
    {
        if (attackingTeamId == teamId)
        {
            // Just reinforcements
            Debug.Log("Reinforcements have arrived");
            numberOfShips += attackingNumShips;
            UpdateTooltip();
            return;
        }

        // Compute attacker's power and defender's power.
        // Multiple ships combine super-linearly, so it's (slightly) advantagous so bundle your ships in a single massive fleet.
        var attSizeMultiplier = Mathf.Log10(attackingNumShips) / 3.0f + 0.25;
        var defSizeMultiplier = Mathf.Log10(numberOfShips) / 3.0f + 0.25;
        var attPower = attSizeMultiplier * attackingEffectiveness;
        var defPower = defSizeMultiplier * shipEffectiveness;

        Debug.Log("Being attacked! Stats: "+attackingNumShips+" "+numberOfShips+" "+attackingEffectiveness+" "+shipEffectiveness+" "+attPower + " " + defPower);

        // Play the repeatedly-remove-one-from-each-side game
        while (attackingNumShips > 0 && numberOfShips > 0)
        {
            var attackWinProb = attPower / (attPower + defPower);
            if (Random.value > attackWinProb)
            {
                attackingNumShips--;
            }
            else
            {
                numberOfShips--;
            }
        }

        if (numberOfShips == 0)
        {
            // They won
            teamId = attackingTeamId;
            numberOfShips = attackingNumShips;
            shipEffectiveness = shipEffectiveness * 0.95f; // Drop the effectiveness a bit
            DisplayTeamColor();
        }
        else
        {
            // We won, moral improves effectiveness a bit
            shipEffectiveness = Mathf.Min(shipEffectiveness * 1.01f, 1.0f);
        }
        UpdateTooltip();
    }

    private void StartTouching(object sender, InteractableObjectEventArgs e)
    {
        isBeingTouched = true;
    }

    private void StopTouching(object sender, InteractableObjectEventArgs e)
    {
        isBeingTouched = false;
    }

    // Returns true if we are successfully selected (isBeingTouched == true), false otherwise
    public bool TrySelecting()
    {
        if (isBeingTouched)
        {
            var player = playerObject.GetComponent<Player>();
            if (player.isInSelectTargetMode())
            {
                if (player.getSource() == gameObject)
                {
                    // We're just unselecting ourselves
                    DisplayTeamColor();
                    player.setSource(null);
                    return true;
                }
                player.setTarget(gameObject);
            }
            else
            {
                if (player.teamId == teamId)
                {
                    DisplaySelected();
                    player.setSource(gameObject);
                }
            }

            return true;
        }
        return false;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (hasAuthority || (!isServer && !isClient))
        {
            constructionCooldown -= Time.deltaTime;
            if (constructionCooldown < 0)
            {
                numberOfShips++;
                constructionCooldown = constructionTime + Mathf.Log10(numberOfShips);
                UpdateTooltip();
            }
        }
    }
}
