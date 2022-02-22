﻿using System;
using System.Collections.Generic;
using TUNING;

namespace FUtility
{
    public static class BuildingUtil
    {
        public static void AddToPlanScreen(string ID, string menu, string subCategory = "uncategorized", string neighborID = null, bool after = true)
        {
            Log.Debuglog("AddToPlanScreen");

            if(menu.IsNullOrWhiteSpace())
            {
                return;
            }

            if(!neighborID.IsNullOrWhiteSpace())
            {
                var category = BUILDINGS.PLANORDER.Find(x => x.category == menu).buildingAndSubcategoryData;

                if(category is null)
                {
                    return;
                }

                var neighborIndex = category.FindIndex(x => x.Key == neighborID);

                if(neighborIndex != -1)
                {
                    var index = after ? neighborIndex + 1 : neighborIndex - 1;
                    index = Math.Max(index, 0);

                    category.Insert(index, new KeyValuePair<string, string>(ID, subCategory));

                    return;
                }
            }

            ModUtil.AddBuildingToPlanScreen(menu, ID, subCategory);
        }

        public static void AddToResearch(string ID, string tech)
        {
            if (!tech.IsNullOrWhiteSpace())
            {
                Db.Get().Techs.Get(tech).unlockedItemIDs.Add(ID);
            }
        }

        public static BuildingDef CreateTileDef(string ID, string anim, float constructionMass, string material, EffectorValues decor, bool transparent)
        {
            return CreateTileDef(ID, anim, new float[] { constructionMass }, new string[] { material }, decor, transparent);
        }

        public static BuildingDef CreateTileDef(string ID, string anim, float[] constructionMass, string[] material, EffectorValues decor, bool transparent)
        {

            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: anim + "_kanim",
                hitpoints: BUILDINGS.HITPOINTS.TIER1,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                construction_mass: constructionMass,
                construction_materials: material,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER3,
                build_location_rule: BuildLocationRule.Tile,
                decor: decor,
                noise: NOISE_POLLUTION.NONE
                );

            BuildingTemplates.CreateFoundationTileDef(def);
            def.Floodable = false;
            def.Overheatable = false;
            def.Entombable = false;
            def.UseStructureTemperature = false;
            def.AudioCategory = "Glass";
            def.AudioSize = "small";
            def.BaseTimeUntilRepair = -1f;
            def.SceneLayer = transparent ? Grid.SceneLayer.GlassTile : Grid.SceneLayer.TileMain;
            def.isKAnimTile = true;
            def.BlockTileIsTransparent = transparent;
            // def.isSolidTile = true;

            def.BlockTileMaterial = global::Assets.GetMaterial("tiles_solid");

            return def;
        }
    }
}
