namespace Inc.TeamAssistant.Stories.Features;

internal static class CodeForConnectFactory
{
    public static string CreateLight()
    {
        return @"
<svg xmlns=""http://www.w3.org/2000/svg"" version=""1.1"" viewBox=""0 0 45 45"" stroke=""none"">
	<rect width=""100%"" height=""100%"" fill=""#dfdfdf""></rect>
	<path d=""M0,0h7v1h-7z M8,0h1v1h-1z M10,0h1v4h-1z M15,0h2v1h-2z M19,0h1v4h-1z M20,0h1v1h-1z M22,0h1v2h-1z M30,0h1v1h-1z M33,0h1v1h-1z M36,0h1v1h-1z M38,0h7v1h-7z M0,1h1v6h-1z M6,1h1v6h-1z M12,1h1v1h-1z M14,1h1v1h-1z M21,1h1v1h-1z M23,1h1v4h-1z M25,1h2v1h-2z M31,1h1v2h-1z M35,1h1v4h-1z M38,1h1v6h-1z M44,1h1v6h-1z M2,2h3v3h-3z M11,2h1v1h-1z M13,2h1v2h-1z M27,2h2v1h-2z M30,2h1v1h-1z M32,2h2v1h-2z M40,2h3v3h-3z M15,3h3v1h-3z M21,3h2v2h-2z M24,3h1v7h-1z M25,3h1v1h-1z M29,3h1v3h-1z M36,3h1v2h-1z M8,4h1v3h-1z M11,4h2v1h-2z M18,4h1v3h-1z M20,4h1v5h-1z M30,4h2v1h-2z M34,4h1v1h-1z M10,5h1v3h-1z M13,5h5v1h-5z M19,5h1v1h-1z M25,5h1v1h-1z M27,5h1v1h-1z M31,5h1v1h-1z M1,6h5v1h-5z M12,6h1v4h-1z M14,6h1v2h-1z M16,6h1v1h-1z M22,6h1v1h-1z M26,6h1v3h-1z M28,6h1v1h-1z M30,6h1v1h-1z M32,6h1v2h-1z M34,6h1v1h-1z M36,6h1v2h-1z M39,6h5v1h-5z M9,7h1v5h-1z M11,7h1v1h-1z M15,7h1v1h-1z M17,7h1v1h-1z M19,7h1v1h-1z M25,7h1v3h-1z M1,8h7v1h-7z M13,8h1v1h-1z M21,8h3v1h-3z M29,8h2v2h-2z M31,8h1v1h-1z M33,8h1v2h-1z M35,8h1v2h-1z M39,8h2v1h-2z M44,8h1v2h-1z M0,9h1v1h-1z M2,9h1v1h-1z M5,9h1v2h-1z M7,9h2v2h-2z M10,9h1v5h-1z M14,9h1v5h-1z M16,9h1v1h-1z M19,9h1v3h-1z M22,9h2v1h-2z M27,9h2v1h-2z M32,9h1v1h-1z M36,9h2v3h-2z M39,9h1v1h-1z M41,9h1v5h-1z M6,10h1v1h-1z M13,10h1v4h-1z M17,10h2v2h-2z M23,10h1v3h-1z M26,10h2v1h-2z M29,10h1v2h-1z M34,10h1v1h-1z M38,10h1v3h-1z M43,10h1v2h-1z M1,11h2v2h-2z M3,11h1v1h-1z M11,11h1v1h-1z M21,11h2v1h-2z M24,11h1v1h-1z M28,11h1v1h-1z M30,11h1v3h-1z M32,11h1v1h-1z M40,11h1v1h-1z M42,11h1v1h-1z M4,12h1v2h-1z M6,12h3v1h-3z M16,12h1v2h-1z M18,12h1v1h-1z M20,12h2v1h-2z M26,12h1v1h-1z M31,12h1v3h-1z M33,12h1v2h-1z M37,12h1v6h-1z M39,12h1v1h-1z M2,13h1v2h-1z M5,13h1v1h-1z M7,13h2v1h-2z M17,13h1v1h-1z M20,13h1v1h-1z M22,13h1v1h-1z M25,13h1v1h-1z M28,13h2v1h-2z M35,13h2v3h-2z M40,13h1v1h-1z M42,13h1v3h-1z M44,13h1v1h-1z M1,14h1v2h-1z M6,14h2v1h-2z M12,14h1v1h-1z M18,14h2v1h-2z M21,14h1v1h-1z M26,14h2v1h-2z M32,14h1v2h-1z M34,14h1v3h-1z M38,14h1v3h-1z M43,14h1v2h-1z M0,15h1v4h-1z M5,15h1v3h-1z M9,15h1v1h-1z M11,15h1v3h-1z M13,15h3v1h-3z M17,15h1v1h-1z M19,15h1v1h-1z M22,15h4v1h-4z M27,15h4v1h-4z M33,15h1v5h-1z M39,15h3v1h-3z M2,16h1v1h-1z M4,16h1v9h-1z M6,16h3v1h-3z M10,16h1v1h-1z M12,16h1v1h-1z M14,16h3v1h-3z M20,16h2v1h-2z M23,16h1v1h-1z M26,16h2v1h-2z M29,16h3v1h-3z M35,16h1v1h-1z M44,16h1v2h-1z M3,17h1v2h-1z M7,17h1v4h-1z M9,17h1v1h-1z M13,17h1v2h-1z M15,17h1v1h-1z M30,17h2v1h-2z M36,17h1v1h-1z M39,17h4v1h-4z M6,18h1v1h-1z M8,18h1v1h-1z M10,18h1v3h-1z M12,18h1v3h-1z M16,18h1v2h-1z M19,18h1v2h-1z M21,18h2v1h-2z M25,18h2v2h-2z M29,18h1v2h-1z M34,18h2v1h-2z M38,18h1v1h-1z M41,18h3v2h-3z M1,19h2v1h-2z M11,19h1v4h-1z M17,19h2v1h-2z M20,19h1v6h-1z M24,19h1v6h-1z M27,19h1v1h-1z M30,19h3v1h-3z M36,19h1v7h-1z M40,19h1v6h-1z M44,19h1v1h-1z M3,20h1v1h-1z M5,20h2v1h-2z M8,20h1v6h-1z M9,20h1v2h-1z M17,20h1v1h-1z M21,20h3v1h-3z M26,20h1v2h-1z M28,20h1v4h-1z M31,20h1v2h-1z M37,20h3v1h-3z M0,21h1v2h-1z M2,21h1v1h-1z M15,21h2v1h-2z M25,21h1v1h-1z M27,21h1v1h-1z M30,21h1v1h-1z M35,21h1v2h-1z M43,21h2v1h-2z M1,22h1v1h-1z M6,22h1v1h-1z M13,22h2v1h-2z M16,22h1v2h-1z M18,22h2v1h-2z M22,22h1v1h-1z M29,22h1v2h-1z M33,22h1v3h-1z M34,22h1v1h-1z M38,22h1v1h-1z M42,22h1v2h-1z M2,23h2v2h-2z M14,23h1v2h-1z M17,23h2v1h-2z M26,23h1v6h-1z M27,23h1v1h-1z M31,23h1v4h-1z M32,23h1v1h-1z M41,23h1v3h-1z M44,23h1v1h-1z M0,24h2v1h-2z M5,24h3v1h-3z M10,24h2v1h-2z M13,24h1v2h-1z M15,24h1v3h-1z M21,24h1v4h-1z M22,24h2v1h-2z M37,24h2v2h-2z M39,24h1v1h-1z M43,24h1v1h-1z M0,25h1v1h-1z M3,25h1v2h-1z M5,25h1v2h-1z M17,25h1v2h-1z M19,25h1v1h-1z M23,25h1v2h-1z M25,25h1v3h-1z M28,25h3v1h-3z M32,25h1v3h-1z M44,25h1v1h-1z M1,26h1v1h-1z M6,26h2v1h-2z M9,26h1v1h-1z M18,26h1v4h-1z M24,26h1v3h-1z M27,26h1v1h-1z M34,26h1v1h-1z M37,26h1v2h-1z M40,26h1v2h-1z M43,26h1v3h-1z M2,27h1v2h-1z M7,27h2v1h-2z M12,27h1v3h-1z M13,27h1v1h-1z M20,27h1v1h-1z M22,27h1v1h-1z M28,27h1v1h-1z M30,27h1v1h-1z M33,27h1v2h-1z M35,27h2v1h-2z M38,27h1v1h-1z M41,27h2v1h-2z M4,28h1v2h-1z M6,28h2v1h-2z M9,28h1v7h-1z M11,28h1v1h-1z M14,28h2v1h-2z M27,28h1v1h-1z M29,28h1v2h-1z M31,28h1v2h-1z M34,28h1v1h-1z M0,29h2v2h-2z M3,29h1v1h-1z M5,29h1v5h-1z M10,29h1v1h-1z M14,29h1v1h-1z M17,29h1v1h-1z M20,29h1v1h-1z M22,29h1v1h-1z M30,29h1v1h-1z M36,29h2v2h-2z M41,29h1v1h-1z M44,29h1v1h-1z M6,30h2v1h-2z M13,30h1v1h-1z M16,30h1v1h-1z M23,30h1v1h-1z M26,30h1v3h-1z M32,30h1v2h-1z M34,30h1v1h-1z M39,30h1v1h-1z M43,30h1v1h-1z M2,31h3v1h-3z M8,31h1v2h-1z M10,31h2v2h-2z M12,31h1v1h-1z M14,31h2v3h-2z M18,31h4v1h-4z M28,31h1v1h-1z M30,31h2v1h-2z M35,31h1v1h-1z M38,31h1v1h-1z M41,31h1v3h-1z M42,31h1v1h-1z M44,31h1v1h-1z M0,32h1v2h-1z M2,32h1v1h-1z M4,32h1v5h-1z M6,32h1v1h-1z M16,32h1v2h-1z M22,32h2v3h-2z M29,32h1v2h-1z M33,32h1v2h-1z M36,32h1v1h-1z M3,33h1v1h-1z M7,33h1v3h-1z M10,33h1v1h-1z M12,33h1v4h-1z M17,33h3v1h-3z M21,33h1v2h-1z M25,33h1v1h-1z M30,33h2v1h-2z M42,33h1v3h-1z M44,33h1v1h-1z M6,34h1v1h-1z M8,34h1v10h-1z M11,34h1v2h-1z M17,34h1v2h-1z M19,34h1v1h-1z M34,34h1v1h-1z M40,34h1v1h-1z M43,34h1v1h-1z M1,35h3v1h-3z M10,35h1v1h-1z M13,35h1v2h-1z M16,35h1v2h-1z M18,35h1v1h-1z M20,35h1v8h-1z M22,35h1v2h-1z M28,35h1v1h-1z M30,35h4v1h-4z M35,35h1v1h-1z M37,35h3v2h-3z M0,36h1v1h-1z M3,36h1v1h-1z M6,36h1v1h-1z M14,36h1v2h-1z M19,36h1v1h-1z M21,36h1v1h-1z M23,36h4v1h-4z M29,36h2v1h-2z M33,36h2v1h-2z M36,36h1v5h-1z M40,36h1v6h-1z M41,36h1v1h-1z M43,36h1v1h-1z M10,37h2v1h-2z M15,37h1v2h-1z M17,37h2v1h-2z M24,37h1v6h-1z M30,37h1v1h-1z M33,37h1v1h-1z M35,37h1v1h-1z M42,37h1v3h-1z M44,37h1v1h-1z M0,38h7v1h-7z M10,38h1v1h-1z M12,38h1v2h-1z M22,38h1v1h-1z M25,38h5v1h-5z M31,38h1v1h-1z M34,38h1v3h-1z M38,38h1v1h-1z M41,38h1v2h-1z M43,38h1v5h-1z M0,39h1v6h-1z M6,39h1v6h-1z M9,39h1v4h-1z M11,39h1v3h-1z M13,39h1v1h-1z M17,39h1v1h-1z M27,39h2v1h-2z M32,39h1v1h-1z M44,39h1v2h-1z M2,40h3v3h-3z M10,40h1v1h-1z M18,40h1v2h-1z M21,40h2v2h-2z M23,40h1v1h-1z M26,40h1v1h-1z M30,40h2v1h-2z M37,40h3v1h-3z M12,41h1v1h-1z M14,41h1v1h-1z M25,41h1v1h-1z M29,41h2v1h-2z M32,41h2v1h-2z M41,41h2v2h-2z M10,42h1v1h-1z M13,42h1v2h-1z M15,42h2v2h-2z M19,42h1v2h-1z M23,42h1v2h-1z M26,42h1v3h-1z M28,42h1v1h-1z M33,42h4v1h-4z M11,43h1v2h-1z M14,43h1v2h-1z M17,43h1v1h-1z M25,43h1v1h-1z M27,43h1v2h-1z M29,43h2v1h-2z M32,43h1v1h-1z M34,43h1v1h-1z M36,43h2v2h-2z M39,43h1v1h-1z M42,43h1v1h-1z M1,44h5v1h-5z M9,44h2v1h-2z M12,44h1v1h-1z M18,44h1v1h-1z M20,44h1v1h-1z M24,44h1v1h-1z M31,44h1v1h-1z M35,44h1v1h-1z M38,44h1v1h-1z M41,44h1v1h-1z M43,44h1v1h-1z"" fill=""#000""></path>
</svg>";
    }
    
    public static string CreateDark()
    {
        return @"
<svg xmlns=""http://www.w3.org/2000/svg"" version=""1.1"" viewBox=""0 0 45 45"" stroke=""none"">
	<rect width=""100%"" height=""100%"" fill=""#9d9d9d""></rect>
	<path d=""M0,0h7v1h-7z M8,0h1v1h-1z M10,0h1v1h-1z M13,0h2v1h-2z M17,0h1v3h-1z M19,0h2v1h-2z M22,0h2v1h-2z M30,0h1v1h-1z M36,0h1v1h-1z M38,0h7v1h-7z M0,1h1v6h-1z M6,1h1v6h-1z M9,1h1v3h-1z M12,1h1v1h-1z M14,1h1v3h-1z M15,1h1v1h-1z M21,1h2v1h-2z M25,1h2v2h-2z M27,1h1v1h-1z M31,1h1v1h-1z M33,1h1v2h-1z M35,1h1v4h-1z M38,1h1v6h-1z M44,1h1v6h-1z M2,2h3v3h-3z M11,2h1v1h-1z M19,2h1v4h-1z M20,2h1v1h-1z M24,2h1v8h-1z M28,2h1v1h-1z M30,2h1v1h-1z M40,2h3v3h-3z M10,3h1v1h-1z M12,3h1v1h-1z M16,3h1v5h-1z M18,3h1v4h-1z M21,3h3v2h-3z M26,3h2v2h-2z M29,3h1v3h-1z M31,3h2v1h-2z M36,3h1v2h-1z M8,4h1v3h-1z M11,4h1v2h-1z M20,4h1v5h-1z M25,4h1v1h-1z M30,4h1v1h-1z M32,4h1v4h-1z M34,4h1v1h-1z M9,5h2v1h-2z M17,5h1v1h-1z M31,5h1v1h-1z M33,5h1v1h-1z M1,6h5v1h-5z M10,6h1v4h-1z M12,6h1v1h-1z M14,6h1v2h-1z M22,6h1v1h-1z M26,6h1v1h-1z M28,6h1v2h-1z M30,6h1v2h-1z M34,6h1v1h-1z M36,6h1v2h-1z M39,6h5v1h-5z M11,7h1v4h-1z M13,7h1v3h-1z M17,7h1v1h-1z M31,7h1v1h-1z M33,7h1v1h-1z M35,7h1v5h-1z M1,8h7v1h-7z M9,8h1v1h-1z M21,8h3v1h-3z M25,8h1v3h-1z M26,8h1v1h-1z M39,8h2v1h-2z M44,8h1v2h-1z M0,9h1v2h-1z M2,9h1v2h-1z M5,9h1v1h-1z M12,9h1v1h-1z M14,9h1v1h-1z M16,9h1v1h-1z M22,9h1v1h-1z M28,9h3v1h-3z M33,9h1v1h-1z M36,9h2v3h-2z M39,9h1v1h-1z M42,9h1v3h-1z M1,10h1v2h-1z M4,10h1v1h-1z M6,10h2v1h-2z M9,10h1v1h-1z M17,10h2v1h-2z M20,10h1v1h-1z M23,10h1v1h-1z M26,10h1v1h-1z M28,10h1v2h-1z M32,10h1v2h-1z M34,10h1v1h-1z M38,10h1v3h-1z M41,10h1v3h-1z M43,10h1v2h-1z M7,11h1v2h-1z M14,11h1v3h-1z M17,11h1v1h-1z M19,11h1v1h-1z M21,11h1v2h-1z M24,11h1v2h-1z M30,11h2v1h-2z M40,11h1v1h-1z M0,12h1v1h-1z M4,12h1v2h-1z M6,12h1v1h-1z M8,12h1v3h-1z M9,12h1v1h-1z M13,12h1v1h-1z M15,12h2v1h-2z M20,12h1v1h-1z M22,12h2v1h-2z M26,12h1v1h-1z M37,12h1v9h-1z M39,12h1v1h-1z M1,13h1v3h-1z M5,13h1v1h-1z M10,13h1v1h-1z M12,13h1v4h-1z M17,13h2v3h-2z M28,13h1v1h-1z M30,13h1v1h-1z M32,13h2v1h-2z M35,13h2v3h-2z M42,13h1v1h-1z M44,13h1v1h-1z M3,14h1v4h-1z M6,14h1v1h-1z M11,14h1v1h-1z M16,14h1v2h-1z M19,14h1v3h-1z M21,14h1v1h-1z M25,14h3v1h-3z M29,14h1v1h-1z M31,14h1v1h-1z M33,14h1v6h-1z M38,14h3v1h-3z M43,14h1v2h-1z M0,15h1v3h-1z M4,15h2v2h-2z M7,15h1v3h-1z M13,15h2v1h-2z M23,15h2v2h-2z M25,15h1v1h-1z M27,15h2v1h-2z M30,15h1v3h-1z M32,15h1v1h-1z M34,15h1v2h-1z M38,15h1v2h-1z M40,15h1v1h-1z M42,15h1v1h-1z M2,16h1v3h-1z M6,16h1v1h-1z M8,16h2v1h-2z M11,16h1v3h-1z M14,16h2v1h-2z M20,16h1v1h-1z M22,16h1v5h-1z M27,16h1v1h-1z M31,16h1v6h-1z M35,16h1v1h-1z M39,16h1v2h-1z M41,16h1v2h-1z M44,16h1v2h-1z M4,17h1v1h-1z M9,17h1v1h-1z M13,17h1v2h-1z M15,17h1v4h-1z M16,17h2v1h-2z M25,17h1v1h-1z M32,17h1v3h-1z M36,17h1v1h-1z M42,17h1v1h-1z M5,18h2v1h-2z M8,18h1v1h-1z M10,18h1v2h-1z M18,18h2v1h-2z M21,18h1v1h-1z M26,18h1v1h-1z M29,18h1v1h-1z M34,18h2v1h-2z M38,18h1v3h-1z M43,18h1v1h-1z M0,19h1v1h-1z M4,19h1v6h-1z M7,19h1v2h-1z M16,19h2v1h-2z M19,19h2v1h-2z M24,19h1v6h-1z M30,19h1v4h-1z M36,19h1v6h-1z M40,19h1v6h-1z M41,19h2v1h-2z M44,19h1v3h-1z M1,20h1v2h-1z M3,20h1v3h-1z M5,20h2v1h-2z M8,20h1v5h-1z M12,20h3v1h-3z M17,20h1v1h-1z M20,20h1v5h-1z M21,20h1v1h-1z M23,20h1v1h-1z M25,20h1v1h-1z M27,20h1v1h-1z M29,20h1v1h-1z M35,20h1v2h-1z M39,20h1v1h-1z M43,20h1v1h-1z M9,21h1v1h-1z M16,21h1v1h-1z M18,21h1v1h-1z M0,22h1v4h-1z M2,22h1v3h-1z M6,22h1v1h-1z M12,22h2v1h-2z M17,22h1v2h-1z M19,22h1v1h-1z M22,22h1v1h-1z M25,22h1v1h-1z M27,22h1v1h-1z M29,22h1v2h-1z M33,22h2v1h-2z M38,22h1v1h-1z M42,22h2v2h-2z M11,23h1v2h-1z M28,23h1v1h-1z M31,23h1v5h-1z M32,23h2v1h-2z M41,23h1v3h-1z M3,24h1v3h-1z M5,24h3v1h-3z M10,24h1v1h-1z M14,24h1v1h-1z M18,24h1v2h-1z M21,24h3v2h-3z M25,24h1v4h-1z M26,24h2v1h-2z M34,24h2v1h-2z M37,24h3v1h-3z M43,24h1v2h-1z M5,25h1v1h-1z M7,25h1v1h-1z M13,25h1v2h-1z M15,25h2v1h-2z M28,25h3v1h-3z M32,25h1v2h-1z M37,25h1v1h-1z M39,25h1v2h-1z M44,25h1v1h-1z M1,26h1v2h-1z M6,26h1v1h-1z M8,26h2v2h-2z M16,26h1v1h-1z M19,26h1v1h-1z M21,26h1v4h-1z M26,26h1v3h-1z M34,26h1v1h-1z M36,26h1v1h-1z M2,27h1v1h-1z M4,27h2v2h-2z M12,27h1v2h-1z M15,27h1v4h-1z M17,27h2v1h-2z M20,27h1v3h-1z M22,27h3v1h-3z M27,27h2v1h-2z M30,27h1v1h-1z M33,27h1v1h-1z M35,27h1v1h-1z M37,27h2v1h-2z M41,27h2v1h-2z M0,28h1v1h-1z M6,28h2v1h-2z M9,28h3v1h-3z M18,28h2v2h-2z M24,28h1v2h-1z M29,28h1v2h-1z M34,28h1v1h-1z M44,28h1v2h-1z M3,29h1v1h-1z M7,29h3v1h-3z M14,29h1v1h-1z M30,29h2v1h-2z M33,29h1v1h-1z M35,29h1v1h-1z M37,29h1v1h-1z M41,29h1v1h-1z M1,30h2v1h-2z M6,30h1v1h-1z M22,30h2v1h-2z M26,30h1v1h-1z M34,30h1v1h-1z M36,30h1v1h-1z M39,30h1v1h-1z M43,30h1v1h-1z M0,31h1v2h-1z M2,31h1v1h-1z M5,31h1v1h-1z M9,31h3v1h-3z M14,31h1v1h-1z M18,31h2v1h-2z M21,31h1v1h-1z M25,31h1v3h-1z M28,31h2v1h-2z M31,31h3v1h-3z M35,31h1v1h-1z M37,31h2v1h-2z M41,31h2v1h-2z M1,32h1v1h-1z M3,32h2v1h-2z M6,32h1v1h-1z M8,32h1v1h-1z M10,32h1v3h-1z M15,32h2v2h-2z M19,32h1v1h-1z M22,32h2v5h-2z M27,32h1v2h-1z M29,32h1v3h-1z M33,32h1v2h-1z M36,32h1v3h-1z M38,32h1v1h-1z M41,32h1v1h-1z M43,32h1v1h-1z M2,33h2v1h-2z M7,33h1v4h-1z M9,33h1v1h-1z M11,33h2v1h-2z M17,33h2v1h-2z M20,33h2v1h-2z M26,33h1v1h-1z M30,33h3v1h-3z M42,33h1v1h-1z M44,33h1v1h-1z M4,34h1v3h-1z M6,34h1v1h-1z M8,34h1v2h-1z M18,34h1v3h-1z M19,34h1v1h-1z M21,34h1v1h-1z M24,34h1v7h-1z M31,34h1v1h-1z M34,34h1v4h-1z M37,34h1v1h-1z M40,34h1v1h-1z M43,34h1v1h-1z M1,35h3v1h-3z M11,35h1v2h-1z M14,35h1v3h-1z M16,35h1v2h-1z M26,35h1v3h-1z M28,35h1v1h-1z M30,35h1v1h-1z M32,35h1v1h-1z M35,35h1v1h-1z M38,35h2v2h-2z M41,35h1v3h-1z M42,35h1v1h-1z M0,36h1v1h-1z M3,36h1v1h-1z M6,36h1v1h-1z M9,36h1v2h-1z M20,36h1v6h-1z M21,36h1v1h-1z M25,36h1v1h-1z M31,36h1v5h-1z M33,36h1v1h-1z M36,36h1v5h-1z M37,36h1v1h-1z M40,36h1v6h-1z M43,36h1v1h-1z M8,37h1v7h-1z M12,37h1v1h-1z M19,37h1v1h-1z M29,37h2v1h-2z M42,37h1v3h-1z M44,37h1v1h-1z M0,38h7v1h-7z M10,38h1v2h-1z M15,38h1v1h-1z M17,38h2v2h-2z M22,38h1v1h-1z M27,38h2v2h-2z M33,38h1v1h-1z M38,38h1v1h-1z M43,38h1v3h-1z M0,39h1v6h-1z M6,39h1v6h-1z M13,39h1v1h-1z M16,39h1v1h-1z M25,39h1v2h-1z M29,39h2v1h-2z M34,39h2v1h-2z M44,39h1v2h-1z M2,40h3v3h-3z M11,40h2v2h-2z M15,40h1v1h-1z M18,40h1v1h-1z M21,40h3v2h-3z M26,40h1v1h-1z M30,40h1v2h-1z M34,40h1v1h-1z M37,40h3v1h-3z M41,40h1v1h-1z M14,41h1v2h-1z M16,41h2v3h-2z M33,41h1v2h-1z M42,41h1v3h-1z M9,42h1v2h-1z M15,42h1v2h-1z M19,42h1v1h-1z M23,42h3v1h-3z M28,42h2v1h-2z M31,42h1v1h-1z M34,42h3v1h-3z M41,42h1v1h-1z M43,42h1v1h-1z M10,43h2v1h-2z M13,43h1v1h-1z M18,43h1v1h-1z M20,43h1v1h-1z M22,43h1v1h-1z M26,43h2v1h-2z M29,43h1v1h-1z M32,43h1v1h-1z M34,43h1v1h-1z M36,43h2v2h-2z M39,43h1v1h-1z M1,44h5v1h-5z M12,44h1v1h-1z M14,44h1v1h-1z M16,44h1v1h-1z M21,44h1v1h-1z M25,44h1v1h-1z M28,44h1v1h-1z M31,44h1v1h-1z M35,44h1v1h-1z M38,44h1v1h-1z M41,44h1v1h-1z M43,44h1v1h-1z"" fill=""#000""></path>
</svg>";
    }
}