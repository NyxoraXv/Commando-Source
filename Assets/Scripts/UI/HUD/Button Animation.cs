using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GenericButtonAnimation : MonoBehaviour
{
    public Button[] buttons;

    void Start()
    {
        // Attach the animation function to all buttons in the array
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => AnimateButton(button.gameObject));
        }
    }

    void AnimateButton(GameObject originalButton)
    {
        // Duplicate the button GameObject
        GameObject duplicatedButton = Instantiate(originalButton, originalButton.transform.parent);

        // Set the duplicated button's position to match the original button
        duplicatedButton.transform.position = originalButton.transform.position;

        // Get the Image component of the duplicated button
        Image duplicatedImage = duplicatedButton.GetComponent<Image>();

        // Disable the raycast on the duplicated button's Image
        duplicatedImage.raycastTarget = false;

        // Set the duplicated button's scale to 0.8f
        duplicatedButton.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // Use Dotween to animate the duplicated button
        duplicatedButton.transform.DOScale(1.2f, 0.5f); // Tween scale from 0.8f to 1.2f over 0.5 seconds
        duplicatedImage.DOFade(0f, 0.5f).OnComplete(() => Destroy(duplicatedButton)); // Fade from 1f to 0f over 0.5 seconds

        // You can add additional animations or actions here if needed
    }
}
