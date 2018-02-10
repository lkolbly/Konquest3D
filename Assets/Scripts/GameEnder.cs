using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private int CheckWinCondition() {
        // Check all the planets - if all are owned by one player, that player wins!
        var winningTeam = -1;
        foreach (GameObject planetObj in Object.FindObjectsOfType<GameObject>())
        {
            var planet = planetObj.GetComponent<Planet>();

            // Not even a planet!
            if (planet == null)
            {
                continue;
            }

            // Neutral team can't win
            if (planet.teamId == 0)
            {
                continue;
            }

            winningTeam = winningTeam == -1 ? planet.teamId : winningTeam;
            if (planet.teamId != winningTeam)
            {
                return -1;
            }
        }
        return winningTeam;
    }

    void Update()
    {
        var winningTeam = CheckWinCondition();
        if (winningTeam != -1)
        {
            Debug.Log("Winning team is " + winningTeam);
        }
    }
}
