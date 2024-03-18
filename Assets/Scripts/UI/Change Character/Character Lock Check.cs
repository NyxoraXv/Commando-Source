using UnityEngine;

public class CharacterLockCheck : MonoBehaviour
{
    private void OnEnable()
    {
        // Find the child GameObject named "Lock"
        GameObject lockObject = transform.Find("Lock").gameObject;

        // Check character ownership and set the active state of the "Lock" GameObject accordingly
        Character card = this.GetComponent<EnumCard>().Character;

        if (CharacterManager.Instance.ownedCharacters.TryGetValue(card, out int index))
        {
            lockObject.SetActive(false); // Deactivate "Lock" GameObject if character is owned
        }
        else
        {
            lockObject.SetActive(true); // Activate "Lock" GameObject if character is not owned
        }
    }
}
