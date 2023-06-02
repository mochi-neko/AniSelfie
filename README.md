# AniSelfie

Takes a selfie with virtual camera wearing your virtual avatar ([VRM](https://vrm.dev/en/)) with motion tracking ([mocopi](https://www.sony.net/Products/mocopi-dev/en/)).

This repository is now beta version.

## Introduction

1. Clone this repository.
2. Open the project with Unity ver2022.3.0f1.
3. Download mocopi Receiver Plugin ver1.0.3 for Unity from [here](https://www.sony.net/Products/mocopi-dev/en/downloads/DownloadInfo.html#Unity_Plugin).
4. Import .unitypackage to this project. (Minimum requirement is only `Runtime` folder.)
5. Open scene: `Assets/AniSelfie/Scenes/AniSelfie.unity`.

## How to use

1. Specify your VRM model file path to `Vrm Model File Path` field on [Operator](./Assets/Mochineko/AniSelfie/Operator.cs).
2. Specify port using on mocopi to `Mocopi Port` field on [Operator](./Assets/Mochineko/AniSelfie/Operator.cs). (Default port is `12351`.)
3. Begin to record motion on mocopi or [BVH Sender](https://sony.net/Products/mocopi-dev/en/downloads/DownloadInfo.html#BVH_Sender) with specifying port and IP address of your PC that runs Unity to mocopi app or BVH Sender.
4. Press `Play` button on Unity Editor.
5. Adjust virtual camera rotation and zoom by pressing arrow keys `←`, `→`, `↑` and `↓`.
6. Take pose for virtual camera by virtual avatar and mocopi tracking as you like.
7. Take selfie with your virtual avatar by pressing `C` key (instant capture) or `D` key (capture after 5 seconds).
8. Saved images are in `<ProjectDirectory>/Selfies/`. (This directory is git ignored.)

If you do not have mocopi device, you can use BVH motion file by [BVH Sender](https://sony.net/Products/mocopi-dev/en/downloads/DownloadInfo.html#BVH_Sender).

## 3rd Party Notices

See [NOTICE](./NOTICE.md).

If you use mocopi SDK, please check [commercial license agreement](https://www.sony.net/Products/mocopi-dev/en/others/Licence.html) and eula in SDK file.

## License

Licensed under the [MIT](./LICENSE) license.