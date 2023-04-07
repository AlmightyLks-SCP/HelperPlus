# OUTDATED
This Plugin was intended for Synapse 2, and is not made, nor compatible with Synapse3.

---

# HelperPlus
A plugin helping out devs debugging their own plugins and servers.  
The UI updates ca. twice a second.  
The ram usage may be inaccurate to a degree because of how the way windows allocates memory per-process, see [here](https://docs.microsoft.com/en-us/windows/win32/memory/working-set).

---

### Command

| Name | Description | Usage | Console Type | Permission  | 
| ------------- | ------------- | ------------- | ------------- | ------------- |
| pHelper | Toggle a UI for plugin debugging | "phelper" | Remote Admin | helperplus.phelper
| clear | Clear items, ragdolls and/or dummies | "clear [all/ragdolls/items/dummies]" | Remote Admin | helperplus.clear

---

### Config

Self-explanatory.  

Default:
```
[HelperPlus]
{
server:
  enabled: false
  displayServerFps: false
  displayTotalRamUsage: false
  displaySLRamUsage: false
environment:
  enabled: false
  displayPosition: false
  displayCurrentRoom: false
  displayTargetName: false
  displayTargetPosition: false
items:
  enabled: false
  displayTotalItems: false
  displayWeaponAmount: false
  displayGrenadeAmount: false
  displayMedicalAmount: false
  displayKeycardAmount: false
map:
  enabled: false
  displayDoorAmount: false
  displayRoomAmount: false
  displayRagdollAmount: false
  displayDummyAmount: false
}
```

