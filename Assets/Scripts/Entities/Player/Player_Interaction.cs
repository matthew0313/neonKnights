using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player_Interaction : MonoBehaviour, ICutsceneCallbackReceiver
{
    [SerializeField] TMP_Text interactionText;
    [SerializeField] float m_interactionRange;
    public float interactionRange => m_interactionRange;

    List<Interaction> interactions = new List<Interaction>();
    bool cutscenePaused = false;
    public void OnUpdate()
    {
        if (interactions.Count > 0)
        {
            if (cutscenePaused) return;
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
            interactionText.gameObject.SetActive(true);
            interactionText.transform.position = Camera.main.WorldToScreenPoint(interactions[0].transform.position);
            interactionText.text = $"[E] {interactions[0].interactionName.text}";

            // E키로 상호작용 실행
            if (InputManager.GetInteractInput())
            {
                interactions[0].OnInteract();
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
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

    public void OnCutsceneStart()
    {
        cutscenePaused = true;
        interactionText.gameObject.SetActive(false);
    }

    public void OnCutsceneEnd()
    {
        cutscenePaused = false;
    }
}