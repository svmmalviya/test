﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using InterviewProject;

namespace InterviewProject.Controllers
{
    [Route("{controller}")]
    public class EmpsController : Controller
    {
        private InterviewProjectDBEntities db = new InterviewProjectDBEntities();

        // GET: Emps
        public ActionResult Index()
        {

            return PartialView("Index", db.Emps.Select(x => new MyEmp
            {
                id = x.id,
                city = x.cityid,
                contact = db.contacts.Where(y => (int)y.empid == x.id).FirstOrDefault().phone,
                email = db.contacts.Where(y => (int)y.empid == x.id).FirstOrDefault().email,
                CountryValue = db.countries.Where(y => y.id == x.countryid).FirstOrDefault().country1,
                StateValue = db.states.Where(y => y.id == x.stateid).FirstOrDefault().state1,
                CityValue = db.cities.Where(y => y.id == x.cityid).FirstOrDefault().city1,
                DOB = (DateTime)x.DOB,
                firstname = x.firstname,
                lastname = x.lastname,
                state = x.stateid
            }).ToList());
        }

        // GET: Emps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyEmp emp = db.Emps.Where(x => x.id == id).Select(x => new MyEmp
            {
                id = x.id,
                city = x.cityid,
                contact = db.contacts.Where(y => (int)y.empid == x.id).FirstOrDefault().phone,
                email = db.contacts.Where(y => (int)y.empid == x.id).FirstOrDefault().email,
                CountryValue = db.countries.Where(y => y.id == x.countryid).FirstOrDefault().country1,
                StateValue = db.states.Where(y => y.id == x.stateid).FirstOrDefault().state1,
                CityValue = db.cities.Where(y => y.id == x.cityid).FirstOrDefault().city1,
                DOB = (DateTime)x.DOB,
                firstname = x.firstname,
                lastname = x.lastname,
                state = x.stateid
            }).FirstOrDefault();
            if (emp == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", emp);
        }

        // GET: Emps/Create
        [Route("~/")]
        [Route("")]
        [Route("Lists")]
        public ActionResult Create()
        {
            ViewBag.countries = db.countries.Select(x => new countries { id = x.id, country = x.country1 }).ToList();
            return View();
        }

        // POST: Emps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,firstname,lastname,DOB,contactid,countryid,stateid,cityid")] Emp emp)
        public ActionResult Create([Form] MyEmp emp)
        {
            ViewBag.countries = db.countries.Select(x => new countries { id = x.id, country = x.country1 }).ToList();
            if (ModelState.IsValid)
            {
                //db.contacts.Add(new contact {email= })
                db.Emps.Add(new Emp
                {
                    firstname = emp.firstname,
                    lastname = emp.lastname,
                    DOB = emp.DOB,
                    contactid = 1,
                    countryid = emp.country,
                    stateid = emp.state,
                    cityid = emp.city,
                });

                db.SaveChanges();

                var storedEmp = db.Emps.Where(x => x.firstname.Trim() == emp.firstname.Trim() && x.lastname.Trim() == emp.lastname.Trim() && x.DOB == emp.DOB).FirstOrDefault();

                var contact = db.contacts.Add(new contact { email = emp.email, phone = emp.contact, empid = storedEmp.id });

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(emp);
        }

        public ActionResult States(int country)
        {
            var db = new InterviewProjectDBEntities();
            var cities = db.states.Where(x => x.cid == country).Select(x => new states { id = x.id, state = x.state1 }).ToList();
            return PartialView("States", cities);
        }

        public ActionResult Cities(int state)
        {
            var db = new InterviewProjectDBEntities();
            var cities = db.cities.Where(x => x.sid == state).Select(x => new cities { id = x.id, city = x.city1 }).ToList();
            return PartialView("Cities", cities);
        }

        // GET: Emps/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.countries = db.countries.Select(x => new countries { id = x.id, country = x.country1 }).ToList();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyEmp emp = db.Emps.Where(x=>x.id==id).Select(x => new MyEmp
            {
                id = x.id,
                city = x.cityid,
                contact = db.contacts.Where(y => (int)y.empid == x.id).FirstOrDefault().phone,
                email = db.contacts.Where(y => (int)y.empid == x.id).FirstOrDefault().email,
                country = x.countryid,
                DOB = (DateTime)x.DOB,
                firstname = x.firstname,
                lastname = x.lastname,
                state = x.stateid
            }).FirstOrDefault();

            if (emp == null)
            {
                return HttpNotFound();
            }

            ViewBag.states = db.states.Where(x => x.cid == emp.country).Select(x => new states { id = x.id, state = x.state1 }).ToList();
            ViewBag.cities = db.cities.Where(x => x.sid == emp.state).Select(x => new cities { id = x.id, city = x.city1 }).ToList();

            return PartialView("Edit", emp);
        }

        // POST: Emps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MyEmp emp)
        {
            ViewBag.countries = db.countries.Select(x => new countries { id = x.id, country = x.country1 }).ToList();
            if (ModelState.IsValid)
            {
                var toUopdate = db.Emps.Find(emp.id);

                if (toUopdate == null) return HttpNotFound();



                toUopdate.firstname = emp.firstname;
                toUopdate.lastname = emp.lastname;
                toUopdate.DOB = emp.DOB;
                toUopdate.stateid = emp.state;
                toUopdate.cityid = emp.city;
                toUopdate.contactid = 0;
                toUopdate.countryid = emp.country;


                db.SaveChanges();
                var contacts = db.contacts.Where(c => c.empid == emp.id).FirstOrDefault();

                contacts.empid = emp.id; contacts.email = emp.email; contacts.phone = emp.contact;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.states = db.states.Where(x => x.cid == emp.country).Select(x => new states { id = x.id, state = x.state1 }).ToList();
            ViewBag.cities = db.cities.Where(x => x.sid == emp.state).Select(x => new cities { id = x.id, city = x.city1 }).ToList();
            return Json("1");
        }

        // GET: Emps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp emp = db.Emps.Find(id);

            if (emp == null)
            {
                return HttpNotFound();
            }

            db.Emps.Remove(emp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Emps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Emp emp = db.Emps.Find(id);
            db.Emps.Remove(emp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class cities
    {
        public int id { get; set; }
        public string city { get; set; }
    }
    public class countries
    {
        public int id { get; set; }
        public string country { get; set; }
    }

    public class states
    {
        public int id { get; set; }
        public string state { get; set; }
    }
    public class MyEmp
    {
        public int? id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Date of Birthday")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [EmailAddress(ErrorMessage = "invalid email address")]
        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; }
        [MaxLength(10, ErrorMessage = "Maximum length.")]
        public string contact { get; set; }
        [Required(ErrorMessage = "Select country")]
        public int country { get; set; }
        [Required(ErrorMessage = "Select state")]
        public int state { get; set; }
        [Required(ErrorMessage = "Select city")]
        public int city { get; set; }

        public string CountryValue { get; set; }
        public string StateValue { get; set; }
        public string CityValue { get; set; }

    }
}
