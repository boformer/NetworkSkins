Network Skins
=============

Mod for Cities: Skylines

Harmony Dependency
------------------

**Uses a custom build of Harmony 1.2.1.0, found in the libs folder!** 

The build is based on this [Harmony commit](https://github.com/pardeike/Harmony/tree/0d2bcc42917f0a1c0e58543e90ccb908038bc0ce), with the following change to `HarmonyInstance.UnpatchAll`:

```csharp
public void UnpatchAll(string harmonyID = null)
{
	bool IDCheck(Patch patchInfo) => harmonyID == null || patchInfo.owner == harmonyID;

	var originals = GetPatchedMethods().ToList();
	foreach (var original in originals)
	{
		var info = GetPatchInfo(original);
		info.Postfixes.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.patch));
		info.Prefixes.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.patch));
		info.Transpilers.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.patch));
	}
}
```

That version of Harmony fixes issues with certain transpiler patches that were present in 1.2.0.1, the change to `UnpatchAll` is required for the unpatching to work correctly (postfixes must be reverted before prefixes if they are using a `__state` parameter).
