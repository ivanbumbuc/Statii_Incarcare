namespace Statii_Incarcare.Models
{
    public static class NextBackCalendar
    {
        public static int zile=0;

        public static int Back()
        {
            zile--;
            return zile;
        }
        public static int Next()
        {
            zile++;
            return zile;
        }
    }
}
