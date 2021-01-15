# *Just 1 Int ⇒ 7 Toggles (J1I7T)* 🗡️

*Just 1 Int 7 Toggles* was an Unity Editor tool that would let you use just 1 Expression Parameter to create a menu with 7 Toggles.

**In January 2021, VRChat has pushed an update that introduced synced booleans to Avatars 3.0,
allowing avatars to contain much more synced parameters  🎉**

This update makes J1I7T redundant. For this reason, this project will no longer be maintained! 😃

### Convert existing J1I7T Avatars 3.0 to Native Avatars 3.0

If your avatar still uses an old version of J1I7T, the newest version of J1I7T contains a [conversion tool](https://github.com/hai-vr/Just1Int7Toggles/releases)
which allows you to quickly get rid of J1I7T in order to replace it with VRChat's native synced booleans.

This conversion tool should only be used in projects that already have J1I7T avatars.

Select the J1I7T component, and click the big *Convert* button. This will allow you to:

- Assign a Parameter Name to a Toggle
- Automatically create the required Expression Parameters
- Automatically clean up empty Expression Parameters

Once this is set, click the `Generate FX Animator layers` button.
This removes the binary tree animator layers, animator parameters, and creates new animator layers.

Once this is done, you will still need to create the Expression menus to toggle the items by yourself.
