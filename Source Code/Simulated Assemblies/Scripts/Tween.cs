using UnityEngine;

public static class Tween
{
    // Linear interpolation
    public static float Linear(float t)
    {
        return t;
    }

    // Quadratic easing in - accelerating from zero velocity
    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    // Quadratic easing out - decelerating to zero velocity
    public static float EaseOutQuad(float t)
    {
        return t * (2f - t);
    }

    // Quadratic easing in/out - acceleration until halfway, then deceleration
    public static float EaseInOutQuad(float t)
    {
        if (t < 0.5f)
            return 2f * t * t;
        else
            return -1f + (4f - 2f * t) * t;
    }

    // Back easing out - overshooting the target value and settling back
    public static float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
    }

    // Custom easing: bounce backwards first, then accelerate towards the target
    public static float BounceBackEaseIn(float t)
    {
        float bounceDistance = 200.0f; // How far the object moves away from the target
        float bounceDuration = 0.3f; // Duration of the bounce-back phase

        if (t < bounceDuration)
        {
            // Calculate backward motion during the bounce-back phase
            float normalizedT = t / bounceDuration; // Normalize time for the bounce phase
            return -bounceDistance * (1f - normalizedT * normalizedT); // Smooth backward motion (quadratic ease-out)
        } else
        {
            // Accelerate towards the target during the forward motion phase
            float forwardT = (t - bounceDuration) / (1f - bounceDuration); // Normalize time for the forward phase
            return forwardT * forwardT; // Smooth forward acceleration (quadratic ease-in)
        }
    }

    // More easing functions can be added here...
}

