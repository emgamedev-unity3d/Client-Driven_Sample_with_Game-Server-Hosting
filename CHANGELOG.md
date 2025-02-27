# Change log

## [unreleased] - yyyy-mm-dd
N/A

## [1.2.1] - 2023-02-17

### Dynamic Addressables Network Prefabs

#### Changed
- Readme updated with link to the Dynamic Addressables Network Prefabs sample documentation, and broken links fixed (#106)

### 2D Space Shooter

#### Changed
- Readme updated with link to the 2D Space Shooter sample documentation, and broken links fixed (#106)

### Client Driven

#### Changed
- Readme updated with link to the Client Driven sample documentation, and broken links fixed (#106)

### Invaders

#### Changed
- Readme updated with link to the Invaders samples documentation, and broken links fixed (#106)

### Bitesize Samples Repository

#### Fixed
- Readme formatting adjustments and broken link fixes (#106) (#108)

## [1.2.0] - 2023-02-16

### Dynamic Addressables Network Prefabs Sample

Added the Dynamic Addressables Network Prefabs Sample. This sample showcases the available use-cases for the dynamic prefab system, which allows us to add new spawnable prefabs at runtime. This would be useful for games trying to use both [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@0.8/manual/index.html) and Netcode for GameObjects (#63) (#94) (#96) (#97) (#98) (#99) (#100) (#101) (#103)

### 2DSpaceShooter

#### Fixed
- Resolved issue where the colour of powerups was not displayed correctly (#91)
- Removing warning when spawning powerups (#90). Fixed the order in which powerups were spawned and when their NetworkVariable value was initialized. Now they are spawned beforehand.
- Fixing bullet explosion desync (#89). Bullet explosion vfx were happening too early on clients because of NetworkTransform's interpolation. Bullets are now no longer synchronised by NetworkTransforms and instead only have their velocity set through client rpcs when they are spawned. Since they are no longer interpolated, they are not lagging behind the server and are at the correct position when they receive the despawn message from the server.
- Fix: Broken Reference for Underline Character in Font Asset (#87)

#### Changed
- Upgraded sample to 2021.3.18f1 LTS (#91)
- Readme was updated (#96)
- Added readme banner image (#101)


### Client Driven

#### Changed
- Upgraded sample to 2021.3.18f1 LTS (#91)
- Readme was updated (#96)
- Added readme banner image (#101)

### Invaders

#### Changed
- Upgraded sample to 2021.3.18f1 LTS (#91)
- Readme was updated (#96)
- Added readme banner image (#101)

### Bitesize Samples Repository
#### Changed
- Readme was updated (#96)


## [1.1.0] - 2022-12-13

### Client Driven

#### Changed
- Changed connection UI to use UI buttons and events instead of OnGUI [MTT-4201] (#60)
- ThirdPersonController starter assets added (#62)
- Uniformize gitignore files (#65)
- Art Pass (#68)
- Anti Aliasing (#72)
- Third Person Character Controller integration with custom IP UI (#66) The sample has been refactored to feature a PlayerPrefab inside NetworkManager that is a networked variant of a prefab from [Unity's Starter Assets](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526). Other improvements include:
  - Input polling via the new Input System
  - Connection UI utilizes UI Toolkit
  - Updated to Unity 2021.3.15f1 LTS
- Change URP grading mode to HDR (#73)
- Player Colors (#75)
- UI Art (#69)
- Readme added (#76)
- Upgrade to Netcode for GameObjects v1.2.0 & cleaned up in-scene NetworkVariables (#78)
- Ingredient spawn position offset (#81)
- In-game UI backgrounds (#82)
- Initial position sync fix on owning clients (#85)

### 2DSpaceShooter

#### Changed
- UI Pass using UI Toolkit (#55)
- Uniformize gitignore files (#65)
- 2DSpaceShooter & Invaders 2021.3.15f1 LTS update & Readmes (#77)
- Updating Invaders and 2DSpaceShooter to Netcode for GameObjects v1.2.0 (#84)

### Invaders

#### Fixed
- Fixing NotServerException [MTT-4029] (#59)
- Despawning enemies instead of destroying them (#74)

#### Changed
- Updated to Unity 2021.3.15f1 LTS
- 2DSpaceShooter & Invaders 2021.3.15f1 LTS update & Readmes (#77)
- Updating Invaders and 2DSpaceShooter to Netcode for GameObjects v1.2.0 (#84)

## [1.0.0] - 2021-10-20

### Client Driven

A new sample was added named client driven. It focus on client driven movements, networked physics, spawning vs statically placed objects, object reparenting

### Invaders

#### Changed
- Updated to Netcode for GameObjects 1.0.0.
- Player now use ClientNetworkTransform for client driven movement
- Network Manager now uses Unity Transport instead of UNet
- SceneTransitionHandler : now uses the new Scene Manager and Scene Loading events
- InvadersGame : replicated time remaining now uses a RPC instead of a one time synchonized NetworkVariable

#### Fixed
- Network behaviour in OnDestroy was moved to OnNetworkDespawn

### 2DSpaceShooter

#### Changed
- Updated to Netcode for GameObjects 1.0.0.
- Player name is no longer static but based on the client id.

#### Fixed
- Fixed a bug where explosions from bullet impacts where only visible on the host.
- Fixed a bug where setting the hosts port in the UI wouldn't change the port on which the server was hosted.

## [0.2.0] - 2021-07-21

### Invaders

#### New Changes

- Game - rename all our alien prefabs to have a more generic name, the same principle was applied to our codebase, renamed variables/fields/classes to something more generic
- Game: Fix a crash in shipping build
- Enemies: Rename our main enemy class to EnemyAgent + minor clean-ups + implement a grace shoot timer period
- InvadersGame: Some big refactories here, the UpdateEnemies function not outputs a set of flags (bitmask) rather than having separated booleans to keep track off
- InvadersGame: Fix an edge case of the game loop where if the enemies would reach the bottom they would never respawn, now when they do reach that bottom boundary it will be game over
- InvadersGame: Introduce additional game over reasons
- LobbyControl: Introduce a minimum player count variable that could be tweaked in the inspector so that the users can start playing in the editor with just the host in the lobby
- PlayerControl: Unified the NotifyGameOver function with the InvadersGame one + added different texts for all the possible game over reasons to be displayed
- PlayerControl: Fix a minor issue where the Player graphics are not hidden on all connected instances upon "death".

#### Known Issues

- Upon import a MissingReferenceException is triggered from within MLAPI: **"MissingReferenceException: The object of type ‘GameObject’ has been destroyed but you are still trying to access it.
  Your script should either check if it is null or you should not destroy the object."** in:
  - UnityEngine.GameObject.GetComponent[T] () (at /Users/bokken/buildslave/unity/build/Runtime/Export/Scripting/GameObject.bindings.cs:28)

### 2DSpaceShooter

#### New Changes
- Cleaned up project structure, removed duplicate art assets and moved remaining asset into sub folders.
- Removed empty MonoBehaviours which were used as tags and using the Unity tag system instead.
- Removed the dependency to the community contributions repository and copied `NetworkManagerHud` and `NetworkObjectPool` into the sample.
- Small QoL improvements to scripting. Using `TryGetComponent` and `CompareTag`.
- Fix audio issues by moving the audio listener onto the player ship object.

#### Known Issues

- Missing Reference Exception thrown when leaving playmode, can be ignored:<br>
*MissingReferenceException: The object of type 'NetworkObjectPool' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
MLAPI.Extensions.NetworkObjectPool.ReturnNetworkObject*

## [0.1.0] - 2021-04-07

Initial release of MLAPI Bitesize Samples repository. Samples support the following versions:

| Unity Version | Unity MLAPI Version |
| -- | -- |
| 2020.3 | 0.1.0 |

### New features

- Added Invaders sample.
- Added 2D Space Shooter sample.

### License

The Bitesize Samples repository and code projects are licensed under the Unity Companion License for Unity-dependent projects (see https://unity3d.com/legal/licenses/unity_companion_license). See [LICENSE](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.bitesize/blob/main/LICENSE.md) for full details.
