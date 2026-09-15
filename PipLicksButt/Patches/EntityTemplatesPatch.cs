using HarmonyLib;
using PipLicksButt.Content.Scripts;
using UnityEngine;

namespace PipLicksButt.Patches
{
    public class EntityTemplatesPatch
    {
        [HarmonyPatch(typeof(EntityTemplates), "AddCreatureBrain")]
        public class EntityTemplates_AddCreatureBrain_Patch
        {
            public static void Prefix(GameObject prefab, ChoreTable.Builder chore_table, Tag species, string symbol_prefix)
            {
                if (species == GameTags.Creatures.Species.SquirrelSpecies && prefab.TryGetComponent(out Navigator navigator))
                {
                    var isAdult = navigator.NavGridName == GameNavGrids.SQUIRREL; // babies use the baby drecko grid
                    var wantsToLick = !prefab.HasTag(ModTags.doNotLickMyButtPlz);

                    if (isAdult && wantsToLick)
                    {
                        chore_table.PushInterruptGroup();

                        var index = chore_table.infos.FindIndex(info => info.def is CritterEmoteStates.Def);

                        if (index != -1)
                        {
                            chore_table.infos.Insert(index + 1, new ChoreTable.Builder.Info()
                            {
                                interruptGroupId = chore_table.interruptGroupId,
                                forcePriority = ChoreTable.Builder.INVALID_PRIORITY,
                                def = new PipLicksButtStates.Def()
                                {
                                    animName = "plb_pip_licks_butt_kanim",
                                    animSequence = new HashedString[]
                                    {
                                        "lick_butt_pre",
                                        "lick_butt_loop",
                                        "lick_butt_loop",
                                        "lick_butt_loop",
                                        "lick_butt_loop",
                                        "lick_butt_loop",
                                        "lick_butt_pst"
                                    }
                                }
                            });
                        }

                        chore_table.PopInterruptGroup();
                    }
                }
            }
        }
    }
}
