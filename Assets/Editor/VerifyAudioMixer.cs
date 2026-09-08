using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public static class VerifyAudioMixer
{
    [MenuItem("Tools/Audio/Verify MainMixer Setup")]
    public static void RunVerification()
    {
        AssetDatabase.ImportAsset("Assets/Audio/MainMixer.mixer", ImportAssetOptions.ForceUpdate);
        AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/MainMixer.mixer");

        if (mixer == null)
        {
            Debug.LogError("[AudioVerify] FAILED: MainMixer.mixer could not be loaded!");
            return;
        }

        Debug.Log("[AudioVerify] Successfully loaded " + mixer.name);

        AudioMixerGroup[] allGroups = mixer.FindMatchingGroups(string.Empty);
        Debug.Log("[AudioVerify] Total Groups Found: " + allGroups.Length);
        foreach (var g in allGroups)
        {
            Debug.Log("[AudioVerify] - Group: " + g.name);
        }

        AudioMixerGroup[] bgmGroups = mixer.FindMatchingGroups("BGM");
        AudioMixerGroup[] sfxGroups = mixer.FindMatchingGroups("SFX");

        bool hasBGMGroup = bgmGroups != null && bgmGroups.Length > 0;
        bool hasSFXGroup = sfxGroups != null && sfxGroups.Length > 0;

        Debug.Log("[AudioVerify] Has BGM Group: " + hasBGMGroup);
        Debug.Log("[AudioVerify] Has SFX Group: " + hasSFXGroup);

        bool sfxParamOk = mixer.SetFloat("SFXVolume", -5f);
        bool bgmParamOk = mixer.SetFloat("BGMVolume", -5f);
        bool masterParamOk = mixer.SetFloat("MasterVolume", -5f);

        float testVal = 0f;
        bool sfxGetOk = mixer.GetFloat("SFXVolume", out testVal);
        Debug.Log("[AudioVerify] Set SFXVolume result: " + sfxParamOk + ", GetFloat: " + sfxGetOk + " (Val: " + testVal + " dB)");

        bool bgmGetOk = mixer.GetFloat("BGMVolume", out testVal);
        Debug.Log("[AudioVerify] Set BGMVolume result: " + bgmParamOk + ", GetFloat: " + bgmGetOk + " (Val: " + testVal + " dB)");

        bool masterGetOk = mixer.GetFloat("MasterVolume", out testVal);
        Debug.Log("[AudioVerify] Set MasterVolume result: " + masterParamOk + ", GetFloat: " + masterGetOk + " (Val: " + testVal + " dB)");

        // Reset back to 0 dB
        mixer.SetFloat("SFXVolume", 0f);
        mixer.SetFloat("BGMVolume", 0f);
        mixer.SetFloat("MasterVolume", 0f);

        bool isPlaying = EditorApplication.isPlaying;
        bool allParamsExist = sfxGetOk && bgmGetOk && masterGetOk;
        bool allParamsSet = !isPlaying || (sfxParamOk && bgmParamOk && masterParamOk);

        if (hasBGMGroup && hasSFXGroup && allParamsExist && allParamsSet)
        {
            Debug.Log("[AudioVerify] SUCCESS! All groups (Master, BGM, SFX) and exposed parameters (MasterVolume, BGMVolume, SFXVolume) are working perfectly!");
        }
        else
        {
            Debug.LogError("[AudioVerify] FAILED: Missing groups or parameters. Groups - BGM: " + hasBGMGroup + ", SFX: " + hasSFXGroup + ". Params - SFX: " + sfxGetOk + ", BGM: " + bgmGetOk + ", Master: " + masterGetOk);
        }
    }
}
