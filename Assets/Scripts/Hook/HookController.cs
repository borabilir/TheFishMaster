using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class HookController : MonoBehaviour
{
    public Transform hookedTransform;

    private Camera mainCamera;
    private Collider2D collider;

    private int length, strength, fishCount;
    private bool canMove = false;

    private List<Fish> hookedFishes;

    private Tweener cameraTween;

    void Awake()
    {
        mainCamera = Camera.main;
        collider = GetComponent<Collider2D>();
        hookedFishes = new List<Fish>();
    }

    void Update()
    {
        if (canMove && Input.GetMouseButton(0))
        {
            Vector3 vector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position;
            position.x = vector.x;
            transform.position = position;
        }
    }

    public void StartFishing()
    {
        length = IdleManager.instance.length - 20;
        strength = IdleManager.instance.strength;
        fishCount = 0;
        float time = -length * 0.1f;

        cameraTween = mainCamera.transform.DOMoveY(length, 1 + time * 0.25f, false).OnUpdate(delegate
        {
            if (mainCamera.transform.position.y <= -11)
                transform.SetParent(mainCamera.transform);
        }).OnComplete(delegate
        {
            collider.enabled = true;
            cameraTween = mainCamera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {
                if (mainCamera.transform.position.y >= -25f)
                    StopFishing();
            });
        });

        ScreenManager.instance.ChangeScreen(Screens.Game);
        collider.enabled = false;
        canMove = true;
        hookedFishes.Clear();
    }

    private void StopFishing()
    {
        canMove = false;
        cameraTween.Kill(false);
        cameraTween = mainCamera.transform.DOMoveY(0, 2, false)
            .OnUpdate(delegate
        {
            if (mainCamera.transform.position.y >= -11)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, -6);
            }
        })
            .OnComplete(delegate
        {
            transform.position = Vector2.down * 6;
            collider.enabled = true;
            int totalPrice = 0;
            for (int i = 0; i < hookedFishes.Count; i++)
            {
                hookedFishes[i].transform.SetParent(null);
                hookedFishes[i].ResetFish();
                totalPrice += hookedFishes[i].Type.price;
            }
            IdleManager.instance.totalGain = totalPrice;
            ScreenManager.instance.ChangeScreen(Screens.End);
        });

    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Fish") && fishCount != strength)
        {
            fishCount++;
            Fish component = target.GetComponent<Fish>();
            component.Hooked();
            hookedFishes.Add(component);
            target.transform.SetParent(transform);
            target.transform.position = hookedTransform.position;
            target.transform.rotation = hookedTransform.rotation;
            target.transform.localScale = Vector3.one;
            target.transform.DOShakeRotation(5, Vector3.forward * 45, 10, 90).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {
                target.transform.rotation = Quaternion.identity;
            });
            if (fishCount == strength)
                StopFishing();
        }
    }

}
