using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RR.EditorTools
{
    /// <summary>
    /// Settings, cena e builds por script (padrao ProjectSetup/BuildAndroid do COE). Idempotente. Em batch:
    ///   Unity -batchmode -quit -projectPath client -executeMethod RR.EditorTools.Setup.Apply
    ///   Unity -batchmode -quit -projectPath client -executeMethod RR.EditorTools.Setup.BuildWindows (ou BuildAndroidDev)
    /// A cena e' artefato gerado e vazia: o Game nasce sozinho (RuntimeInitializeOnLoadMethod).
    /// </summary>
    public static class Setup
    {
        // ponytail: appId provisorio; depois de publicado na Play nao muda nunca (decisao do idealizador)
        public const string AppId = "br.com.vstack.runerelay";
        const string Scene = "Assets/_RR/Scenes/Main.unity";

        [MenuItem("RR/Aplicar settings")]
        public static void Apply()
        {
            PlayerSettings.companyName = "V-STACK";
            PlayerSettings.productName = "Rune Relay";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, AppId);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, AppId);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.runInBackground = true;

            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;      // a Play exige 64 bits
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26; // ponytail: hipotese ate fixar o aparelho minimo
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.predictiveBackSupport = true; // API 36: o voltar chega ao jogo como Esc (lido pelo Input legado, ver activeInputHandler)

            // Windows so' de dev: janela retrato para jogar e fotografar no PC.
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.resizableWindow = true;

            // 2 = Input System + Input Manager legado (sem API publica; vale na proxima abertura do editor, como no COE). O legado e' so
            // para o voltar do Android: com o GameActivity o Input System nao recebe a tecla (Forge Street, emulador API 35; forum Unity 1555368)
            var ps = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            SerializedProperty handler = ps.FindProperty("activeInputHandler");
            if (handler != null && handler.intValue != 2) { handler.intValue = 2; ps.ApplyModifiedPropertiesWithoutUndo(); }

            if (!File.Exists(Scene))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Scene));
                EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single), Scene);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Scene, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("RR Setup: ok (" + AppId + ", retrato, Android ARM64 IL2CPP API 26+, Input System)");
        }

        public static void BuildWindows() => Build(BuildTarget.StandaloneWindows64, "Builds/win/RuneRelay.exe");
        public static void BuildAndroidDev() => Build(BuildTarget.Android, "Builds/android/RuneRelay-dev.apk");
        // Emulador do PC (x86_64): o berberis do Android 15 derruba o ARM64 traduzido do Unity/IL2CPP (Forge Street, 2026-10-08).
        // ponytail: APK so de teste local; aparelho e Play recebem so ARM64 (BuildAndroidDev).
        public static void BuildAndroidEmu() => Build(BuildTarget.Android, "Builds/android/RuneRelay-emu.apk", AndroidArchitecture.X86_64);

        static void Build(BuildTarget target, string path, AndroidArchitecture arch = AndroidArchitecture.None)
        {
            Apply();
            if (arch != AndroidArchitecture.None) PlayerSettings.Android.targetArchitectures = arch;
            // o empacotamento incremental do Gradle reaproveita o APK anterior: no Forge deixou ~12 MB de buracos e, alternando
            // ARM64/x86_64, saiu APK sem libunity/libil2cpp (crash "libgame.so not found"). Apagar a saida custa ~1 min
            const string gradleOut = "Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build";
            if (target == BuildTarget.Android && Directory.Exists(gradleOut)) Directory.Delete(gradleOut, true);
            BuildReport r = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { Scene },
                locationPathName = path,
                target = target,
                options = BuildOptions.Development,
            });
            Debug.Log($"BuildSummary({target}): result={r.summary.result} errors={r.summary.totalErrors} size={r.summary.totalSize} path={path}");
            if (arch != AndroidArchitecture.None) { PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64; AssetDatabase.SaveAssets(); }   // nao vaza para o build do aparelho
            if (Application.isBatchMode) EditorApplication.Exit(r.summary.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}
