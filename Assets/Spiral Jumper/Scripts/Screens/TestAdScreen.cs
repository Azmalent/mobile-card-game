using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpiralJumper
{
    public class TestAdScreen : MonoBehaviour
    {
        public Text nameText;
        public Text counter;
        public DiGro.UButton cancelButton;
        public DiGro.UButton revardButton;

        public bool tmp_show = false;

        private bool m_isRevarded = false;

        private float m_time = -100;
        private float m_timeToCancel = -100;

        private Action m_onCancelAction = null;
        private Action m_onRevardAction = null;


        private void Awake()
        {
            nameText.text = "";
            counter.text = "";
            cancelButton.gameObject.SetActive(false);
            revardButton.gameObject.SetActive(false);

            cancelButton.OnClick.AddListener(OnCancelButtonClick);
            revardButton.OnClick.AddListener(OnRevardButtonClick);
        }

        private void Update()
        {
            if(tmp_show)
            {
                tmp_show = false;
                ShowRewardedAd(30, null, null);
            }

            if (!m_isRevarded && m_timeToCancel > -100)
            {
                m_timeToCancel -= Time.deltaTime;
                if (m_timeToCancel <= 0)
                {
                    m_timeToCancel = -100;
                    cancelButton.gameObject.SetActive(true);
                }
            }

            if (m_time > -100)
            {
                m_time -= Time.deltaTime;
                if (m_time <= 0)
                {
                    m_time = -100;
                    counter.text = "";
                    if (m_isRevarded)
                    {
                        cancelButton.gameObject.SetActive(false);
                        revardButton.gameObject.SetActive(true);
                    } else
                    {
                        cancelButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    counter.text = Math.Round(m_time, 1).ToString();
                }
            }
        }


        public void ShowRewardedAd(float time, Action onCancelAction, Action onRevardAction)
        {
            m_time = time;

            nameText.text = "Rewarded Ad";
            counter.text = Math.Round(m_time, 1).ToString();
            cancelButton.gameObject.SetActive(true);
            revardButton.gameObject.SetActive(false);

            m_onCancelAction = onCancelAction;
            m_onRevardAction = onRevardAction;

            m_isRevarded = true;
        }

        public void ShowAd(float time, float timeToCancel, Action onCancelAction)
        {
            m_time = time;
            m_timeToCancel = timeToCancel < 0 ? 0 : timeToCancel;

            nameText.text = "Not Rewarded Ad";
            counter.text = Math.Round(m_time, 1).ToString();
            revardButton.gameObject.SetActive(false);

            m_onCancelAction = onCancelAction;

            m_isRevarded = false;
        }

        private void OnCancelButtonClick()
        {
            m_onCancelAction?.Invoke();
            Destroy(gameObject);
        }

        private void OnRevardButtonClick()
        {
            m_onRevardAction?.Invoke();
            Destroy(gameObject);
        }
    }
}