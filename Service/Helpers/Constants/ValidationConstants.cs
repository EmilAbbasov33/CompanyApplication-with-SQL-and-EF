namespace Service.Helpers.Constants
{
    public static class ValidationConstants
    {
        public const int MinAge = 18;
        public const int MaxAge = 60;
        public const string DepartmentNotFoundMessage = "Belə bir departament yoxdur, başqa departament seçin";
        public const string EmployeeNotFoundMessage = "Belə bir işçi tapılmadı";
        public const string InvalidAgeMessage = "Yaş 18 ilə 60 arasında olmalıdır";
        public const string InvalidNameMessage = "Ad/Soyad yalnız hərflərdən ibarət olmalıdır, rəqəm və ya xüsusi simvol ola bilməz";
        public const string RequiredFieldMessage = "Bu sahə mütləq doldurulmalıdır";
        public const string NegativeCapacityMessage = "Capacity mənfi ola bilməz";
        public const string DepartmentNameMustContainLetterMessage = "Department adı ən azı bir hərf saxlamalıdır";
        public const string DepartmentNameExistsMessage = "Bu adda departament artıq mövcuddur";
        public const string DepartmentFullMessage = "'{0}' departamenti dolubdur (tutum: {1}), yeni işçi əlavə edilə bilməz";
        public const string CapacityTooSmallMessage = "Capacity {0}-dan kiçik ola bilməz, çünki departamentdə hazırda {0} işçi var";
        public const string DepartmentCapacityZeroMessage = "'{0}' departamentinin tutumu 0 olduğu üçün bura işçi əlavə edilə bilməz";
    }
}
