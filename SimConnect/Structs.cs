using System.Runtime.InteropServices;

namespace MSFSFlightFollowing.Models
{
   public class SimConnectStructs
   {
      public enum DEFINITIONS
      {
         AircraftStatus
      }

      public enum DATA_REQUEST
      {
         AircraftStatus
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
      public struct AircraftStatusStruct
      {
         public double Latitude;
         public double Longitude;
         public double Altitude;
         public double CurrentFuel;
         public double TotalFuel;
         public double TrueHeading;
         public double AirspeedIndicated;
         public double AirspeedTrue;

         public bool NavHasSignal;
         public bool NavHasDME;
         public double DMEDistance;
         public bool GPSFlightPlanActive;
         public bool GPSWaypointModeActive;
         public int GPSWaypointIndex;
         public double GPSWaypointDistance;
         public double GPSNextWPLatitude;
         public double GPSNextWPLongitude;
         public double GPSPrevWPLatitude;
         public double GPSPrevWPLongitude;
         public double GPSWPETE;

         public bool AutopilotAvailable;
         public bool AutopilotMaster;
         public bool AutopilotWingLevel;
         public bool AutopilotAltitude;
         public bool AutopilotApproach;
         public bool AutopilotBackcourse;
         public bool AutopilotFlightDirector;
         public bool AutopilotAirspeed;
         public bool AutopilotMach;
         public bool AutopilotYawDamper;
         public bool AutopilotAutothrottle;
         public bool AutopilotVerticalHold;
         public bool AutopilotHeading;
         public bool AutopilotNav1;
      }
   }
}
