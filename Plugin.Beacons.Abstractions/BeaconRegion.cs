using System;


namespace Plugin.Beacons
{

    public class BeaconRegion
    {
        public BeaconRegion(string identifier, Guid uuid, ushort? major = null, ushort? minor = null)
        {
            this.Identifier = identifier;
            this.Uuid = uuid;
            this.Major = major;
            this.Minor = minor;
        }


        public string Identifier { get; }
        public Guid Uuid { get; }
        public ushort? Major { get; }
        public ushort? Minor { get; }


        public bool IsBeaconInRegion(Beacon beacon)
        {
            if (!this.Uuid.Equals(beacon.Uuid))
                return false;

            if (this.Major == null && this.Minor == null)
                return true;

            if (this.Major != beacon.Major)
                return false;

            if (this.Minor == null || this.Minor == beacon.Minor)
                return true;

            return false;
        }


        public override string ToString()
            => $"[Identifier: {this.Identifier} - UUID: {this.Uuid} - Major: {this.Major ?? 0} - Minor: {this.Minor ?? 0}]";


        public override bool Equals(object obj)
        {
            var other = obj as BeaconRegion;
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (this.Major != other.Major)
                return false;

            if (this.Minor != other.Minor)
                return false;

            if (this.Uuid != other.Uuid)
                return false;

            return true;
        }


        public override int GetHashCode()
        {
            var hash = this.Identifier.GetHashCode();
            if (this.Uuid != null)
                hash += this.Uuid.GetHashCode();

            if (this.Major != null)
                hash += this.Major.Value.GetHashCode();

            if (this.Minor != null)
                hash += this.Minor.Value.GetHashCode();

            return hash;
        }
    }
}