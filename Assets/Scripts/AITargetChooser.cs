using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AITargetChooser : MonoBehaviour
{
    public string PlayerTag = "Player";
    private Transform PlayerTarget;
    public string CrystalTag = "Crystal";
    private Transform CrystalTarget;
    public string WallsLayerName;
    public string TargetsLayerName;
    public float LineOfSightTargetMultiplier;
    private AIDestinationSetter targetRef;
    private AIPath path;
    private RaycastHit2D ray;
    private LayerMask layer;
    private int targetLayer;
    private Character character;

    void Start()
    {
        targetRef = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        character = GetComponent<Character>();
        targetLayer = LayerMask.NameToLayer(TargetsLayerName);
        layer = (1 << LayerMask.NameToLayer(WallsLayerName)) | (1 << targetLayer);
        PlayerTarget = GameObject.FindWithTag(PlayerTag).transform;
        CrystalTarget = GameObject.FindWithTag(CrystalTag).transform;
    }

    void Update()
    {
        // Choose target
        if (Vector2.Distance(transform.position, CrystalTarget.position) < path.endReachedDistance || Vector2.Distance(transform.position, PlayerTarget.position) > path.endReachedDistance * LineOfSightTargetMultiplier)
            targetRef.target = CrystalTarget;
        else
            targetRef.target = PlayerTarget;

        // Check for line of sight with target
        ray = Physics2D.Raycast(transform.position, targetRef.target.position - transform.position, distance: path.endReachedDistance, layerMask: layer);
        if (path.reachedEndOfPath && ray && ray.collider.gameObject.layer != targetLayer)
        {
            Debug.DrawRay(transform.position, (targetRef.target.position - transform.position).normalized * path.endReachedDistance, Color.red);
            path.whenCloseToDestination = CloseToDestinationMode.ContinueToExactDestination;
            character.HasLineOfSight = false;
        }
        else
        {
            Debug.DrawRay(transform.position, (targetRef.target.position - transform.position).normalized * path.endReachedDistance, path.reachedEndOfPath ? Color.yellow : Color.red);
            path.whenCloseToDestination = CloseToDestinationMode.Stop;
            character.HasLineOfSight = path.reachedEndOfPath;
        }

        // Always look towards target
        transform.rotation = Quaternion.LookRotation(Vector3.forward, targetRef.target.position - transform.position);
    }
}
