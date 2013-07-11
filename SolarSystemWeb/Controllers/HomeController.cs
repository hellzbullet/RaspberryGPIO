using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;
using NHibernate;
using RaspberryGPIO.GPIO;
using SolarSystem.Log;
using SolarSystem.Utils;
using SolarSystemDatabase.Database;
using SolarSystemDatabase.Models;

namespace SolarSystemWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISession _session;

        public HomeController()
        {
            _session = NHibernateConfig.SessionFactory.Value.OpenSession(); 
        }

        public ActionResult Index()
        {
            HttpEncoder.Current = HttpEncoder.Default;
            var list = _session.CreateCriteria<Device>().List<Device>().ToList();
            return View(list);
        }

        public ActionResult Start(int Id)
        {
            using (var t = _session.BeginTransaction()) {
                var device = _session.CreateCriteria<Device>().List<Device>().SingleOrDefault(x => x.Id == Id);
                device.Start(true);
                _session.SaveOrUpdate(device);
                t.Commit();
                new Thread(SystemCycle.DoCycle).Start();
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Stop(int Id)
        {
            using (var t = _session.BeginTransaction())
            {
                var device = _session.CreateCriteria<Device>().List<Device>().SingleOrDefault(x => x.Id == Id);
                device.Stop(true);
                _session.SaveOrUpdate(device);
                t.Commit();
                new Thread(SystemCycle.DoCycle).Start();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Automatic(int Id)
        {
            using (var t = _session.BeginTransaction())
            {
                var device = _session.CreateCriteria<Device>().List<Device>().SingleOrDefault(x => x.Id == Id);
                device.IsUserControlled = false;
                _session.SaveOrUpdate(device);
                t.Commit();
                new Thread(SystemCycle.DoCycle).Start();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Manual(int Id)
        {
            using (var t = _session.BeginTransaction())
            {
                var device = _session.CreateCriteria<Device>().List<Device>().SingleOrDefault(x => x.Id == Id);
                device.IsUserControlled = true;
                _session.SaveOrUpdate(device);
                t.Commit();
                new Thread(SystemCycle.DoCycle).Start();
            }
            return RedirectToAction("Index");
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (_session != null && _session.IsOpen) {
                _session.Dispose();
            }
            base.OnActionExecuted(filterContext);
        }

        public ActionResult AddDevice()
        {
            return View(new Device());
        }

        public ActionResult Logs()
        {
            return View(_session.CreateCriteria<LogEntry>().List<LogEntry>());
        }

        [HttpPost]
        public ActionResult AddDevice(Device device)
        {
            if (device.DevicePower != 0 && String.IsNullOrEmpty(device.Name) == false && device.Priority != 0 && device.Pin != 0 && GPIOPinFactory.UsedPins.Keys.Contains((uint)device.Pin) == false) {
                using (var t = _session.BeginTransaction()) {
                    _session.Save(device);
                    t.Commit();
                    Logger.Instance.Value.Log("Device added - " + device.Name + " " + device.DevicePower);
                }
                new Thread(SystemCycle.DoCycle).Start();
                return RedirectToAction("Index");
            }

            ViewData["error"] = "ERROR";
            return View(device);
        }

        public JsonResult PowerData()
        {
            var list = _session.CreateCriteria<Power>().List<Power>().Where(x => new DateTime(x.Date) >= DateTime.Today).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
