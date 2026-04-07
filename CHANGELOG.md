# 2.0.0

### Breaking Changes

- `IIdContainer` interface removed. Inherit from `SO_IdContainer` instead.
- `IdAssigner<T>` renamed to `IdAssignerWindow<T>`. Update subclass declarations accordingly.
- `IdAssignerWindow<T>` constraint changed from `T : Object, IIdContainer` to `T : SO_IdContainer`.

### New

- `SO_IdContainer` — abstract `ScriptableObject` base class that implements ID storage, serialization, and `IdSet` tracking. Replaces the manual `IIdContainer` boilerplate.

---

# 1.0.0

Initial release.
