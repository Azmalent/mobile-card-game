using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace DiGro {

    public class ColorHolder : MonoBehaviour {
        [SerializeField] private List<SpriteRenderer> m_sprites = new List<SpriteRenderer>();
        [SerializeField] private List<Image> m_images = new List<Image>();
        [SerializeField] private List<Text> m_texts = new List<Text>();

        public bool overrideAlfa = true;
        public PalletColor palletColor = PalletColor.Empty;
        public Pallete LocalPallete { get; set; } = null;


        private void Awake() {
            ColorTheme.get.OnPalleteChange += UpdateColor;
        }

        private void OnDestroy() {
            if (ColorTheme.HasInstance)
                ColorTheme.get.OnPalleteChange -= UpdateColor;
        }

        private void Start() {
            UpdateColor();
        }

        public void SetColor(PalletColor color) {
            palletColor = color;
            UpdateColor();
        }

        public void UpdateColor() {
            if (LocalPallete == null)
                UpdateColor(ColorTheme.GetColor(palletColor));
            else
                UpdateColor(LocalPallete.GetColor(palletColor));
        }

        private void UpdateColor(Color color) {
            if (m_sprites.Count == 0 && m_images.Count == 0 && m_texts.Count == 0)
                return;

            foreach (var sprite in m_sprites) {
                sprite.color = new Color {
                    r = color.r,
                    g = color.g,
                    b = color.b,
                    a = overrideAlfa ? color.a : sprite.color.a
                };
            }
            foreach (var image in m_images) {
                image.color = new Color {
                    r = color.r,
                    g = color.g,
                    b = color.b,
                    a = overrideAlfa ? color.a : image.color.a
                };
            }
            foreach (var text in m_texts) {
                text.color = new Color {
                    r = color.r,
                    g = color.g,
                    b = color.b,
                    a = overrideAlfa ? color.a : text.color.a
                };
            }
        }

    }

}