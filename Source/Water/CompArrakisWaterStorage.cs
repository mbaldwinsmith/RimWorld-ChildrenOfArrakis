using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Simple placeholder water storage for catchbasins until full hydration routing is implemented.
    /// </summary>
    public class CompArrakisWaterStorage : ThingComp
    {
        public float storedWater;

        private ThingComp dbhStorageComp;
        private Type dbhStorageType;
        private FieldInfo dbhWaterField;
        private PropertyInfo dbhPropsProperty;
        private FieldInfo dbhCapField;
        private object dbhPropsInstance;

        public CompProperties_ArrakisWaterStorage Props => (CompProperties_ArrakisWaterStorage)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            SyncInitialWater(respawningAfterLoad);
        }

        private void SyncInitialWater(bool respawningAfterLoad)
        {
            float capacity = GetCapacity();
            float current = GetStored();

            if (!respawningAfterLoad && current <= 0f && Props.initialWater > 0f)
            {
                SetStored(Math.Min(Props.initialWater, capacity));
            }
            else if (respawningAfterLoad && storedWater > 0f && current <= 0f)
            {
                // Load legacy saves that only tracked storedWater.
                SetStored(Math.Min(storedWater, capacity));
            }
        }

        public bool HasWater(float amount) => GetStored() >= amount;

        public bool HasSpace(float amount) => GetStored() + amount <= GetCapacity();

        public bool TryConsume(float amount)
        {
            if (!HasWater(amount))
            {
                return false;
            }

            SetStored(GetStored() - amount);
            return true;
        }

        public bool TryAddWater(float amount)
        {
            float capacity = GetCapacity();
            float current = GetStored();
            float toAdd = Math.Min(amount, Math.Max(0f, capacity - current));
            if (toAdd <= 0f)
            {
                return false;
            }

            SetStored(current + toAdd);
            return true;
        }

        public override string CompInspectStringExtra()
        {
            string baseString = base.CompInspectStringExtra();
            float current = GetStored();
            float capacity = GetCapacity();
            string waterLine = $"Stored water: {current:F1} / {capacity:F1}";

            if (Props.sipAmount > 0f)
            {
                waterLine += $" (sip size {Props.sipAmount:F1})";
            }

            if (string.IsNullOrEmpty(baseString))
            {
                return waterLine;
            }

            return baseString + "\n" + waterLine;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            storedWater = GetStored();
            Scribe_Values.Look(ref storedWater, "storedWater", 0f);
        }

        private float GetStored()
        {
            if (TryInitDbhStorage())
            {
                object value = dbhWaterField?.GetValue(dbhStorageComp);
                if (value is float f)
                {
                    return f;
                }
            }

            return storedWater;
        }

        private void SetStored(float value)
        {
            float capped = Math.Max(0f, Math.Min(value, GetCapacity()));

            if (TryInitDbhStorage() && dbhWaterField != null)
            {
                dbhWaterField.SetValue(dbhStorageComp, capped);
            }

            storedWater = capped;
        }

        private float GetCapacity()
        {
            if (TryInitDbhStorage())
            {
                if (dbhPropsInstance == null && dbhPropsProperty != null)
                {
                    dbhPropsInstance = dbhPropsProperty.GetValue(dbhStorageComp);
                }

                if (dbhCapField == null && dbhPropsInstance != null)
                {
                    dbhCapField = dbhPropsInstance.GetType().GetField("WaterStorageCap", BindingFlags.Public | BindingFlags.Instance);
                }

                if (dbhCapField != null && dbhPropsInstance != null)
                {
                    object capObj = dbhCapField.GetValue(dbhPropsInstance);
                    if (capObj is float capFloat)
                    {
                        return capFloat;
                    }
                }
            }

            return Props.capacity;
        }

        private bool TryInitDbhStorage()
        {
            if (parent == null)
            {
                return false;
            }

            if (dbhStorageType == null)
            {
                dbhStorageType = GenTypes.GetTypeInAnyAssembly("DubsBadHygiene.CompWaterStorage");
            }

            if (dbhStorageType == null)
            {
                return false;
            }

            if (dbhStorageComp == null)
            {
                dbhStorageComp = parent.AllComps.FirstOrDefault(c => dbhStorageType.IsInstanceOfType(c));
            }

            if (dbhStorageComp == null)
            {
                return false;
            }

            dbhWaterField ??= dbhStorageType.GetField("WaterStorage", BindingFlags.Public | BindingFlags.Instance);
            dbhPropsProperty ??= dbhStorageType.GetProperty("Props", BindingFlags.Public | BindingFlags.Instance);

            return dbhWaterField != null;
        }
    }
}
