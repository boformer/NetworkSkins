Network Skins
=============

Mod for Cities: Skylines

Harmony Dependency
------------------

**Uses a local custom build of Harmony 2.0.0.0, found in the libs folder!** 

The build is based on this [Harmony commit](https://github.com/pardeike/Harmony/tree/58dc1823d5970e8251ec6ce0d54c371bc08e26f6), with the following change to `HarmonyInstance.UnpatchAll`:

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
