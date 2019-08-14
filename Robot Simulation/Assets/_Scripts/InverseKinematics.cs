using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A typical error function to minimise
public delegate float ErrorFunction(Vector3 target, float[] solution);

public struct PositionRotation
{
    Vector3 position;
    Quaternion rotation;

    public PositionRotation(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    // PositionRotation to Vector3
    public static implicit operator Vector3(PositionRotation pr)
    {
        return pr.position;
    }
    // PositionRotation to Quaternion
    public static implicit operator Quaternion(PositionRotation pr)
    {
        return pr.rotation;
    }
}

//[ExecuteInEditMode]
public class InverseKinematics : MonoBehaviour
{
    [Header("Joints")]
    public Transform BaseJoint;
    public RobotJoint[] Joints = null;
    // The current angles
    public float[] currentAngles = null;

    [Header("Destination")]
    public Transform Effector;
    [Space]
    public Transform Destination;
    public float DistanceFromDestination;
    private Vector3 target;

    [Header("Inverse Kinematics")]
    [Range(0, 1f)]
    public float simulateGradient = 0.1f; // Used to simulate gradient (degrees)
    [Range(0, 100f)]
    public float gradientDependency = 0.1f; // How much we move depending on the gradient

    [Space()]
    [Range(0, 0.25f)]
    public float StopThreshold = 0.1f; // If closer than this, it stops

    public ErrorFunction ErrorFunction;

    [Space]
    [Range(0, 10)]
    public float OrientationWeight = 0.5f;
    [Range(0, 10)]
    public float TorsionWeight = 0.5f;
    public Vector3 TorsionPenality = new Vector3(1, 0, 0);



    // Use this for initialization
    void Start()
    {
        if (Joints == null)
        {
            GetJoints();
        }
        else
        {
            ErrorFunction = DistanceFromTarget;
        }
    }

    // Receives the joints of the robot that it's connected to.
    public void GetJoints()
    {
        Joints = BaseJoint.GetComponentsInChildren<RobotJoint>();
        currentAngles = new float[Joints.Length];
    }



    // Update is called once per frame
    void Update()
    {
        // Gets the next frame's direction by minusing the destination position with the robot's position. Then the value is normalized.
        Vector3 direction = (Destination.position - transform.position).normalized;
        // Gets the target's position by multiplying the direction by the distance; then minus it from the Destination's position.
        target = Destination.position - direction * DistanceFromDestination;
        // If the effector is above the threshold for stopping, we will approach the target.
        if (ErrorFunction(target, currentAngles) > StopThreshold)
            ApproachTarget(target);

    }

    public void ApproachTarget(Vector3 target)
    {
        // Starts from the end, up to the base
        // Starts from joints[end-2]
        //  so it skips the hand that doesn't move!
        for (int i = Joints.Length - 1; i >= 0; i--)
        {

            // Gradient descent
            float gradient = CalculateGradient(target, currentAngles, i, simulateGradient);
            // Approach target while keeping the constraints in mind.
            currentAngles[i] -= gradientDependency * gradient;
            // Clamp
            currentAngles[i] = Joints[i].ClampAngle(currentAngles[i]);

            // Early termination
            if (ErrorFunction(target, currentAngles) <= StopThreshold)
                break;
        }


        for (int i = 0; i < Joints.Length - 1; i++)
        {
            Joints[i].MoveArm(currentAngles[i]);
        }
    }

    /* Calculates the gradient for the inverse kinematic.
     * It simulates the forward kinematics the i-th joint,
     * by moving it +delta and -delta.
     * It then sees which one gets closer to the target.
     * It returns the gradient (suggested changes for the i-th joint)
     * to approach the target. In range (-1,+1)
     */
    public float CalculateGradient(Vector3 target, float[] currentAngles, int i, float delta)
    {
        // Saves the angle,
        // it will be restored later
        float currentAngle = currentAngles[i];

        // Gradient : [F(x+h) - F(x)] / h
        // Update   : Solution -= LearningRate * Gradient
        float partialGradientFunction = ErrorFunction(target, currentAngles);

        currentAngles[i] += delta;
        float gradientFunctionDistance = ErrorFunction(target, currentAngles);

        float gradient = (gradientFunctionDistance - partialGradientFunction) / delta;

        // Restores
        currentAngles[i] = currentAngle;

        return gradient;
    }

    // Returns the distance from the target, given a solution
    public float DistanceFromTarget(Vector3 target, float[] currentAngles)
    {
        Vector3 point = ForwardKinematics(currentAngles);
        return Vector3.Distance(point, target);
    }


    /* Simulates the forward kinematics,
     * given a solution. */
    public PositionRotation ForwardKinematics(float[] currentAngles)
    {
        Vector3 prevPoint = Joints[0].transform.position;

        // Takes object initial rotation into account
        Quaternion rotation = transform.rotation;
        for (int i = 1; i < Joints.Length; i++)
        {
            // Rotates around a new axis.
            // Compares with the angle and position of the previous joint to figure the rotation point.
            rotation *= Quaternion.AngleAxis(currentAngles[i - 1], Joints[i - 1].Axis);
            // Adds it all together to calculate the next point.
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }

        // The end of the effector
        return new PositionRotation(prevPoint, rotation);
    }
}