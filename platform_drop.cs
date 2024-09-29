using UnityEngine;

public class platform_drop : MonoBehaviour {
    private BoxCollider2D platform_collider;
    void Start() {
        // GetComponent() returns only the first matching component found on the GameObject on which it is called,
        // and the order that the components are checked is not defined.
        // Therefore, if there are more than one of the specified type that could match,
        // and you need to find a specific one, you should use Component.GetComponents()
        // and check the list of components returned to identify the one you want.
        //
        // https://docs.unity3d.com/ScriptReference/Component.GetComponent.html
        BoxCollider2D[] boxes = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D box in boxes) {
            if (!box.isTrigger) {
                platform_collider = box;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // In case something else entered a collider - ignore it.
        if (other.gameObject.tag != "Player") {
            return;
        }

        // If we check player's velocity exactly at the time of collision, it will be 0,
        // so the best solution I found was for a platform to have 2 colliders:
        // one is regular and another IsTrigger that is slightly wider on Y axis.
        //
        // If player just jumped and has positive velocity, then we disable regular collider
        // and player ignores a platform's existence.
        //
        // After player leaves IsTrigger collider, we reset a regular collider back.
        //
        // https://discussions.unity.com/t/eficient-one-way-collider/57771/2
        if (other.GetComponent<Rigidbody2D>().velocity.y > 0) {
            Physics2D.IgnoreCollision(platform_collider, other, true);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }

        // While inside a trigger collider, continuously check if a player can drop through a platform
        // If player drops - disable regular collider until he exits a trigger collider
        if (PlayerController.should_fall) {
            Physics2D.IgnoreCollision(platform_collider, other, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }

        PlayerController.should_fall = false;
        Physics2D.IgnoreCollision(platform_collider, other, false);
    }
}