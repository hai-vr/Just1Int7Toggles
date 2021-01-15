# *Just 1 Int ⇒ 7 Toggles* for Avatars 3.0

### [> Download latest version...](https://github.com/hai-vr/Just1Int7Toggles/releases)

*Just 1 Int 7 Toggles* is an Unity Editor tool that uses just 1 Expression Parameter to create a menu with 7 Toggles.

![](https://github.com/hai-vr/Just1Int7Toggles/raw/z-res-pictures/Documentation/illustration-parameter-menu.png)

This tool is a **proof of concept** to showcase complex animator behaviors.

Avatars have a maximum 16 Expression Parameters, synchronized over the network.
Using 1 Int parameter ranging from 0 to 255, avatars can already have nice sets of features and outfits.

However, some avatars may decide to use several Int parameters to toggle various items or features on the avatar.
As a caveat, this reduces the number of available Expression Parameters for other features.

**Using *Just 1 Int 7 Toggles*, multiple toggleable items or features can be compressed into just one parameter.
These toggles are synced.**

This comes at a cost of animator complexity, and a lack of indication on the menu showing that an item is on or off.

![](https://github.com/hai-vr/Just1Int7Toggles/raw/z-res-pictures/Documentation/illustration-animator.png)

## How to use

Use the *Just 1 Int 7 Toggles Compiler* to configure items to toggle, which will add the layers into your FX animator.

### Just 1 Int 7 Toggles Compiler

- Create a GameObject with a *Just 1 Int 7 Toggles Compiler* component.
- Set the FX animator in the *FX Animator Controller* slot.
- Set the avatar in the *Avatar* slot. The avatar will not be modified: This is only used to generate the animations in order to toggle the items.
- Add an item in slots 1-7:
  - Press + to add a GameObject to toggle in the *Item* slot.
    - Multiple GameObjects can be added per item.
    - The dropdown can be changed from *Normal* to *Inverse*.
      - When the item is turned on, *Normal* GameObjects will show up.
      - When the item is turned off, *Inverse* GameObjects will show up.
  - Check *Default state* to choose if the items is enabled by default after loading the avatar.
    - The *Default state* can be different from the initial state of the GameObjects:
      If animations are blocked by safety settings, *Default state* will be ignored.
- Press *Generate* to generate the layers.

![](https://github.com/hai-vr/Just1Int7Toggles/raw/z-res-pictures/Documentation/inspector-compiler2.png)

Note that adding items is not mandatory. You can use parameters to control your own animator layers, see [Advanced: Animator parameters](#advanced-animator-parameters).

### Configuring the avatar

- In your avatar's Expression Parameters, add `J1I7T_A_Sync` as an *Int*.

![](https://github.com/hai-vr/Just1Int7Toggles/raw/z-res-pictures/Documentation/inspector-parameters.png)

- An example Action menu is available at `Assets/Hai/Just1Int7Toggles/Toggles_1-7.asset`.
- An example Parameters is available at `Assets/Hai/Just1Int7Toggles/Parameters_1-7.asset`.

### Advanced: Complex generator for slots 8-15

**WARNING:** This is a **proof of concept** and it is highly discouraged to use slots 8-15.

A complex generator may be used to enable the slot 8-15.

- In your avatar's Expression Parameters, add `J1I7T_B_Sync` as an *Int*.
- An example Action menu is available at `Assets/Hai/Just1Int7Toggles/Toggles_8-15.asset`.
- An example Parameters is available at `Assets/Hai/Just1Int7Toggles/Parameters_1-15_COMPLEX.asset`.

### Advanced: Animator parameters

*Just 1 Int 7 Toggles* can be used without toggling items in the hierarchy.
For example you can toggle the color of a material in the FX layer, or toggle an animation in the Action layer, as if it were a normal toggle.

If you want to use it to enable avatar features instead, you can use the following parameters in any of the Playable Layers, which will either have the value of 0 or 1:

#### Ints
- Item #1: `J1I7T_A_0` (*Int*)
- Item #2: `J1I7T_A_1` (*Int*)
- Item #3: `J1I7T_A_2` (*Int*)
- Item #4: `J1I7T_A_3` (*Int*)
- Item #5: `J1I7T_A_4` (*Int*)
- Item #6: `J1I7T_A_5` (*Int*)
- Item #7: `J1I7T_A_6` (*Int*)

#### Floats
- Item #1: `J1I7T_A_0F` (*Float*)
- Item #2: `J1I7T_A_1F` (*Float*)
- Item #3: `J1I7T_A_2F` (*Float*)
- Item #4: `J1I7T_A_3F` (*Float*)
- Item #5: `J1I7T_A_4F` (*Float*)
- Item #6: `J1I7T_A_5F` (*Float*)
- Item #7: `J1I7T_A_6F` (*Float*)

#### Ints (*Complex*)
- Item #8: `J1I7T_B_0` (*Int*)
- Item #9: `J1I7T_B_1` (*Int*)
- Item #10: `J1I7T_B_2` (*Int*)
- Item #11: `J1I7T_B_3` (*Int*)
- Item #12: `J1I7T_B_4` (*Int*)
- Item #13: `J1I7T_B_5` (*Int*)
- Item #14: `J1I7T_B_6` (*Int*)
- Item #15: `J1I7T_B_7` (*Int*)

#### Floats (*Complex*)
- Item #8: `J1I7T_B_0F` (*Float*)
- Item #9: `J1I7T_B_1F` (*Float*)
- Item #10: `J1I7T_B_2F` (*Float*)
- Item #11: `J1I7T_B_3F` (*Float*)
- Item #12: `J1I7T_B_4F` (*Float*)
- Item #13: `J1I7T_B_5F` (*Float*)
- Item #14: `J1I7T_B_6F` (*Float*)
- Item #15: `J1I7T_B_7F` (*Float*)
