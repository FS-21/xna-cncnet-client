﻿using System;
using System.Collections.Generic;

namespace DTAClient.domain.Multiplayer
{
    public class PlayerHouseInfo
    {
        public int SideIndex { get; set; }
        public int ColorIndex { get; set; }
        public int StartingWaypoint { get; set; }

        public int RealStartingWaypoint { get; set; }

        public bool IsSpectator { get; set; }

        /// <summary>
        /// Applies the player's side into the information
        /// and randomizes it if necessary.
        /// </summary>
        /// <param name="pInfo">The PlayerInfo of the player.</param>
        /// <param name="map">The selected map.</param>
        /// <param name="sideCount">The number of sides in the game.</param>
        /// <param name="random">Random number generator.</param>
        public void RandomizeSide(PlayerInfo pInfo, Map map, int sideCount, Random random)
        {
            if (pInfo.SideId == 0 || pInfo.SideId == sideCount + 1)
            {
                // The player has selected Random or Spectator

                int sideId;

                while (true)
                {
                    sideId = random.Next(0, sideCount);

                    if (map.CoopInfo == null || !map.CoopInfo.DisallowedPlayerSides.Contains(sideId))
                        break;
                }

                SideIndex = sideId;
            }
            else
                SideIndex = pInfo.SideId - 1; // The player has selected a side
        }

        /// <summary>
        /// Applies the player's color into the information and randomizes
        /// it if necessary. If the color is randomized, it's removed
        /// from the list of available colors.
        /// </summary>
        /// <param name="pInfo">The PlayerInfo of the player.</param>
        /// <param name="freeColors">The list of available (un-used) colors.</param>
        /// <param name="mpColors">The list of all multiplayer colors.</param>
        /// <param name="random">Random number generator.</param>
        public void RandomizeColor(PlayerInfo pInfo, List<int> freeColors, 
            List<MultiplayerColor> mpColors, Random random)
        {
            if (pInfo.ColorId == 0)
            {
                // The player has selected Random for their color

                int randomizedColorIndex = random.Next(0, freeColors.Count);
                int actualColorId = freeColors[randomizedColorIndex];

                ColorIndex = mpColors[actualColorId].GameColorIndex;
                freeColors.RemoveAt(randomizedColorIndex);
            }
            else
            {
                ColorIndex = mpColors[pInfo.ColorId - 1].GameColorIndex;
                freeColors.Remove(pInfo.ColorId - 1);
            }
        }

        /// <summary>
        /// Applies the player's starting location into the information and
        /// randomizes it if necessary. If the starting location is randomized,
        /// the starting location is removed from the list of available starting locations.
        /// </summary>
        /// <param name="pInfo">The PlayerInfo of the player.</param>
        /// <param name="map">The selected map.</param>
        /// <param name="freeStartingLocations">List of free starting locations.</param>
        /// <param name="random">Random number generator.</param>
        /// <returns>True if the player's starting location index exceeds the map's number of starting waypoints,
        /// otherwise false.</returns>
        public bool RandomizeStart(PlayerInfo pInfo, Map map,
            List<int> freeStartingLocations, Random random,
            int fakeStartingLocationCount, List<int> takenStartingLocations)
        {
            if (IsSpectator)
            {
                StartingWaypoint = 90;
                return false;
            }

            if (pInfo.StartingLocation == 0)
            {
                // Randomize starting location

                if (freeStartingLocations.Count == 0) // No free starting locs available
                {
                    RealStartingWaypoint = -1;
                    StartingWaypoint = -1;
                    return true;
                }

                int waypointIndex = random.Next(0, freeStartingLocations.Count);
                RealStartingWaypoint = freeStartingLocations[waypointIndex];
                StartingWaypoint = RealStartingWaypoint;
                freeStartingLocations.Remove(StartingWaypoint);
                return false;
            }

            // Use the player's selected starting location
            RealStartingWaypoint = pInfo.StartingLocation - 1;

            if (takenStartingLocations.Contains(RealStartingWaypoint))
            {
                StartingWaypoint = map.MaxPlayers + fakeStartingLocationCount;
                return true;
            }

            takenStartingLocations.Add(RealStartingWaypoint);

            StartingWaypoint = RealStartingWaypoint;
            return false;
        }
    }
}