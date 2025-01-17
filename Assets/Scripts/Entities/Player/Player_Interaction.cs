using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player_Interaction : MonoBehaviour
{
    private List<Interaction> interactions = new List<Interaction>();

    public GameObject interactionTextPrefab;
    private GameObject currentInteractionText;

    private void Update()
    {
        if (interactions.Count > 0)
        {
            Vector3 mousePos = Input.mousePosition;
            
            interactions.Sort((a, b) =>
            {
                Vector3 screenPosA = Camera.main.WorldToScreenPoint(a.transform.position);
                Vector3 screenPosB = Camera.main.WorldToScreenPoint(b.transform.position);
                float distanceA = Vector2.Distance(mousePos, screenPosA);
                float distanceB = Vector2.Distance(mousePos, screenPosB);
                return distanceA.CompareTo(distanceB);
            });

            // 최상위 상호작용 표시
            if (currentInteractionText == null)
            {
                currentInteractionText = Instantiate(interactionTextPrefab);
            }
            currentInteractionText.transform.position = Camera.main.WorldToScreenPoint(interactions[0].transform.position);
            currentInteractionText.GetComponent<UnityEngine.UI.Text>().text = $"[E] {interactions[0].interactionName}";

            // E키로 상호작용 실행
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactions[0].OnInteract();
            }
        }
        else if (currentInteractionText != null)
        {
            Destroy(currentInteractionText);
        }
    }

    public void AddInteraction(Interaction interaction)
    {
        if (!interactions.Contains(interaction))
        {
            interactions.Add(interaction);
        }
    }

    public void RemoveInteraction(Interaction interaction)
    {
        if (interactions.Contains(interaction))
        {
            interactions.Remove(interaction);
        }
    }
}