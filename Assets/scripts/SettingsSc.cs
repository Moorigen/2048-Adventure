using UnityEngine;
using UnityEngine.UI;

public class SettingsSc : MonoBehaviour {

    #region singleton
    public static SettingsSc instance;
    private void Start() {
        instance = this;
    }
    #endregion

    #region Audio
    public AudioSource audioSrc;
    public void ToggleMusic(bool on) {
        audioSrc.enabled = on;
    }

    public void MusicVolume(float vol) {
        audioSrc.volume = vol;
    }
    #endregion

    #region PromoCode

    public InputField promoIpf;

    public void PromoSubmit() {
        string code = promoIpf.text;
        if (code.StartsWith("++")) {
            if (code.EndsWith("g")) {
                code = code.Substring(2, code.Length - 3);
                int gain = 0;
                int.TryParse(code, out gain);
                Player.instance.GainCoin(gain);
            } else if (code.EndsWith("xp")) {
                code = code.Substring(2, code.Length - 4);
                int gain = 0;
                int.TryParse(code, out gain);
                Player.instance.GainExp(gain);
            }
        }
        promoIpf.text = "";
    }

    #endregion

}
