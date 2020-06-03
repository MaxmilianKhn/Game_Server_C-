
namespace src.Security
{
    public class SecurityFunc
    {
        public static ushort Hash(byte[] array)
        {
            ushort hash = 0xFFFF;
            foreach (byte l1 in array)
            {
                hash = (ushort)(hash ^ l1);
                for (int l2 = 0; l2 < 8; ++l2)
                {
                    hash = (ushort)((hash >> 1) ^ (((hash & 0b1) != 0) ? 0xA001 : 0));
                }
            }
            return hash;
        }
    }
}
