// using UnityEngine;
// using UnityEngine.UI;
// using System.IO;
// using UniGLTF;
// using UniGLTF.SceneExport;

// public class GLTFExporter : MonoBehaviour
// {
//     public Button exportButton;
//     public GameObject exportable;

//     void Start()
//     {
//         if (exportButton != null) {
//             exportButton.onClick.AddListener(() =>
//             {
//                 ExportGLB("model");
//             });
//         } else {
//             Debug.LogWarning("Export button doesn't exist");
//         }
//     }

//     public void ExportGLB(string filename) {
//         if (exportable == null) {
//             Debug.LogError("No export root assigned");
//             return;
//         } 

//         // Export to Downloads folder
//         string downloadPath = Path.Combine(
//             System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
//             "Downloads",
//             $"{filename}.glb"
//         );

//         var settings = new GltfExportSettings
//         {
//         };

//         var data = ExportScene.Export(new[] { exportable }, settings);

//         //Write to .glb
//         using (var stream = File.OpenWrite(downloadPath))
//         {
//             new GltfExporter(data).Export(stream);
//         }

//         Debug.Log($"Download GLB at: {downloadPath}");
//     }
// }