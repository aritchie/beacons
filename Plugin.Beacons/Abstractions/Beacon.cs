﻿using System;
using System.IO;
using System.Linq;


namespace Plugin.Beacons
{
    public class Beacon
    {
        public Beacon(Guid uuid, ushort major, ushort minor, double accuracy, Proximity proximity)
        {
            this.Uuid = uuid;
            this.Major = major;
            this.Minor = minor;
            this.Accuracy = accuracy;
            this.Proximity = proximity;
        }


        public Guid Uuid { get; }
        public ushort Minor { get; }
        public ushort Major { get; }
        public double Accuracy { get; }
        public Proximity Proximity { get; }


        public override string ToString()
        {
            return $"[Beacon: Uuid={this.Uuid}, Major={this.Major}, Minor={this.Minor}]";
        }


        public override bool Equals(object obj)
        {
            var other = obj as Beacon;
            if (other == null)
                return false;

            if (this.Uuid != other.Uuid)
                return false;

            if (this.Major != other.Major)
                return false;

            if (this.Minor != other.Minor)
                return false;

            return true;
        }


        public override int GetHashCode()
        {
            return this.Uuid.GetHashCode() + this.Major.GetHashCode() + this.Minor.GetHashCode();
        }


        /// <summary>
        /// This should not be called by iOS as txpower is not available in the advertisement packet
        /// </summary>
        /// <returns>The beacon.</returns>
        /// <param name="data">Data.</param>
        /// <param name="rssi">Rssi.</param>
        public static Beacon Parse(byte[] data, int rssi)
        {
            var uuidString = BitConverter.ToString(data, 4, 16).Replace("-", String.Empty);
            var uuid = new Guid(uuidString);
            var major = BitConverter.ToUInt16(data.Skip(20).Take(2).Reverse().ToArray(), 0);
            var minor = BitConverter.ToUInt16(data.Skip(22).Take(2).Reverse().ToArray(), 0);
            var txpower = data[24];
            var accuracy = CalculateAccuracy(txpower, rssi);
            var proximity = CalculateProximity(accuracy);

            return new Beacon(uuid, major, minor, accuracy, proximity);
        }


        static byte[] ToBytes(Guid guid)
        {
            var hex = guid
                .ToString()
                .Replace("-", String.Empty)
                .Replace("{", String.Empty)
                .Replace("}", String.Empty)
                .Replace(":", String.Empty)
                .Replace("-", String.Empty);

            var bytes = Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

            return bytes;
        }


        public byte[] ToIBeaconPacket()
        {
            using (var ms = new MemoryStream())
            {
                using (var br = new BinaryWriter(ms))
                {
                    br.Write(76);
                    br.Write(new byte[] { 0, 0, 0 });
                    br.Write(ToBytes(this.Uuid));
                    br.Write(BitConverter.GetBytes(this.Major).Reverse().ToArray());
                    br.Write(BitConverter.GetBytes(this.Minor).Reverse().ToArray());
                    br.Write(0); // tx power
                }
                return ms.ToArray();
            }
        }


        public static double CalculateAccuracy(int txpower, double rssi)
        {
            var ratio = rssi * 1 / txpower;
            if (ratio < 1.0)
                return Math.Pow(ratio,10);

            var accuracy =  0.89976 * Math.Pow(ratio, 7.7095) + 0.111;
            return accuracy;
        }


        public static Proximity CalculateProximity(double accuracy)
        {
            if (accuracy < 0)
                return Proximity.Unknown;

            if (accuracy < 0.5)
                return Proximity.Immediate;

            if (accuracy <= 4.0)
                return Proximity.Near;

            return Proximity.Far;
        }


        public static bool IsIBeaconPacket(byte[] data)
        {
            if (data.Length < 25)
                return false;

            // apple manufacturerID - https://www.bluetooth.com/specifications/assigned-numbers/company-Identifiers
            if (data[0] != 76 || data[1] != 0)
                return false;

            return true;
        }
    }
}