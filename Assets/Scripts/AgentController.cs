using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentCntroller : Agent
{

    [SerializeField] private Transform target;  

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0f,1.1f ,0f);
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            target.localPosition = new Vector3(-6f, 1.1f, 0f);
        }
        if (rand == 1)
        {
            target.localPosition = new Vector3(6f, 1.1f, 0f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveSpeed = 5f;

        transform.localPosition += new Vector3(moveX, 0f) * moveSpeed * Time.deltaTime;

        AddReward(-0.001f); //penalty per step to encourage faster solutions
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            AddReward(3f);
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            SetReward(-3f);
            EndEpisode();
        }
    }
}
