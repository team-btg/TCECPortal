using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TCECPortal.Infrastructure.Extensions;
using TCECPortal.Models;
using TCECPortal.Services.FireBase;

namespace TCECPortal.Controllers
{
    public class Attendance : Controller
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ILogger<Attendance> _logger;
        public Attendance(ILogger<Attendance> logger
            , IFirebaseService firebaseService)
        {
            _logger = logger;
            _firebaseService = firebaseService;
        }
        UserModel user;
        public IActionResult Index()
        {
            user = new UserModel();

            user = HttpContext.Session.GetObject<UserModel>("USER_DETAILS");

            AttendanceQR attendanceQR = new AttendanceQR();

            return View(attendanceQR);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateQRCode(AttendanceQR attendance)
        {
            // Creating object of random class
            Random rand = new Random();

            // Choosing the size of string
            // Using Next() string
            int stringlen = rand.Next(4, 10);
            int randValue;
            string str = "";
            char letter;
            for (int i = 0; i < stringlen; i++)
            {

                // Generating a random number.
                randValue = rand.Next(0, 26);

                // Generating random character by converting
                // the random number into character.
                letter = Convert.ToChar(randValue + 65);

                // Appending the letter to string.
                str = str + letter;
            }

            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();

            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
             
            using (MemoryStream ms = new MemoryStream())
            {
                using(Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);

                    attendance.AttendanceCode = str;
                    attendance.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

                    var postFirebase = await _firebaseService.PostAsync("AttendanceQR", attendance);
                }
            }

            return View("Index", attendance);
        }

        public IActionResult ScanIndex()
        {
            return View("Scan");
        }

        [HttpPost]
        public async Task<IActionResult> ScannedQR(string code)
        {
            AttendanceModel attendance = new AttendanceModel();

            attendance.GeneratedCode = code;
            attendance.UserId = user.UserId;
            attendance.CreatedBy = user.UserId.ToString();
            attendance.AttendanceDate = DateTime.Now;

            return View("Scan");
        }
    }

    public class AttendanceQR
    {
        public string QRCodeImage { get; set; } = "";
        public string AttendanceCode { get; set; }
        public DateTime? ValidUntil { get; set; }
    }
}
