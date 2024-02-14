using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationPopup : MonoBehaviour
{
    private static ConfirmationPopup instance;

    public UIAnimator animator;
    public AnimationClip show;
    public AnimationClip hide;

    public TMPro.TextMeshProUGUI textBody;

    private Action confirmAction = null;
    private Action cancelAction = null;

    private void Start()
    {
        instance = this;
        animator.SetToLastFrame(hide);
        gameObject.SetActive(false);
    }

    public static void Show(string text)
    {
        Show(text, "Confirm", "Cancel", null, null);
    }

    public static void Show(string text, string confirm)
    {
        Show(text, confirm, "Cancel", null, null);
    }

    public static void Show(string text, string confirm, string cancel)
    {
        Show(text, confirm, cancel, null, null);
    }

    public static void Show(string text, Action onConfirm)
    {
        Show(text, "Confirm", "Cancel", onConfirm, null);
    }

    public static void Show(string text, Action onConfirm, Action onCancel)
    {
        Show(text, "Confirm", "Cancel", onConfirm, onCancel);
    }

    public static void Show(string text, string confirm, Action onConfirm)
    {
        Show(text, confirm, "Cancel", onConfirm, null);
    }

    public static void Show(string text, string confirm, Action onConfirm, Action onCancel)
    {
        Show(text, confirm, "Cancel", onConfirm, onCancel);
    }

    public static void Show(string text, string confirm, string cancel, Action onConfirm)
    {
        Show(text, confirm, cancel, onConfirm, null);
    }

    public static void Show(string text, string confirm, string cancel, Action onConfirm, Action onCancel)
    {
        instance.textBody.text = text;
        instance.confirmAction = onConfirm;
        instance.cancelAction = onCancel;

        instance.gameObject.SetActive(true);
        instance.animator.Play(instance.show, null, 1, true);
    }

    public static void Hide()
    {
        instance.animator.Play(instance.hide, () =>
        {
            instance.gameObject.SetActive(false);
        });
    }

    public void OnConfirm()
    {
        Hide();
        confirmAction?.Invoke();
    }

    public void OnCancel()
    {
        Hide();
        cancelAction?.Invoke();
    }
}
