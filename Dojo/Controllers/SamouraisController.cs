using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BO;
using Dojo.Data;
using Dojo.Models;

namespace Dojo.Controllers
{
    public class SamouraisController : Controller
    {
        private Context db = new Context();

        private List<Arme> GetArmesLibres()
        {
            var ArmesUtilisees =  db.Samourais.Where(s => s.Arme != null).Select(s => s.Arme.Id).ToList();
            return db.Armes.Where(a => !ArmesUtilisees.Contains(a.Id)).ToList();
        }

        // GET: Samourais
        public ActionResult Index()
        {
            return View(db.Samourais.ToList());
        }

        // GET: Samourais/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            ViewBag.Potentiel = (samourai.Force + (samourai.Arme == null ? 0 : samourai.Arme.Degats)) * (samourai.ArtsMartiaux.Count() > 0 ? samourai.ArtsMartiaux.Count() : 1);

            return View(samourai);
        }

        // GET: Samourais/Create
        public ActionResult Create()
        {
            var vm = new SamouraiVM { Armes = GetArmesLibres(), ArtMartiaux = db.ArtMartials.ToList() };
            return View(vm);
        }

        // POST: Samourais/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SamouraiVM samouraiVM)
        {
            if (ModelState.IsValid)
            {
                if (samouraiVM.SelectedArme.HasValue)
                {
                    samouraiVM.Samourai.Arme = db.Armes.FirstOrDefault(a => a.Id == samouraiVM.SelectedArme);
                }
                samouraiVM.Samourai.ArtsMartiaux = new List<ArtMartial>();
                foreach (var idArtMartial in samouraiVM.SelectedArtMartiaux)
                {
                    samouraiVM.Samourai.ArtsMartiaux.Add(db.ArtMartials.Find(idArtMartial));
                }

                db.Samourais.Add(samouraiVM.Samourai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            samouraiVM.Armes = GetArmesLibres();
            samouraiVM.ArtMartiaux = db.ArtMartials.ToList();

            return View(samouraiVM);
        }

        // GET: Samourais/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }

            var vm = new SamouraiVM();
            vm.Samourai = samourai;
            vm.Armes = GetArmesLibres();
            vm.ArtMartiaux = db.ArtMartials.ToList();
            vm.SelectedArtMartiaux = samourai.ArtsMartiaux.Select(ar => ar.Id).ToList();

            if(samourai.Arme != null)
            {
                vm.SelectedArme = samourai.Arme.Id;
                vm.Armes.Add(samourai.Arme);
            }

            return View(vm);
        }

        // POST: Samourais/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SamouraiVM samouraiVM)
        {
            if (ModelState.IsValid)
            {
                Samourai samouraiDB = db.Samourais.Find(samouraiVM.Samourai.Id);
                if (samouraiDB.Arme != null)
                {
                    var arme = db.Armes.Find(samouraiDB.Arme.Id);
                    db.Entry(arme).State = EntityState.Modified;
                    samouraiVM.Armes.Add(samouraiDB.Arme);
                }

                samouraiDB.Arme = null;
                if (samouraiVM.SelectedArme.HasValue)
                {
                    samouraiDB.Arme = db.Armes.FirstOrDefault(a => a.Id == samouraiVM.SelectedArme);
                }
                samouraiDB.ArtsMartiaux.Clear();
                if (samouraiVM.SelectedArtMartiaux != null)
                {
                    foreach (var idArtMartial in samouraiVM.SelectedArtMartiaux)
                    {
                        samouraiDB.ArtsMartiaux.Add(db.ArtMartials.Find(idArtMartial));
                    }
                }
                samouraiDB.Nom = samouraiVM.Samourai.Nom;
                samouraiDB.Force = samouraiVM.Samourai.Force;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            samouraiVM.Armes = GetArmesLibres();
            samouraiVM.ArtMartiaux = db.ArtMartials.ToList();

            return View(samouraiVM);
        }

        // GET: Samourais/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            return View(samourai);
        }

        // POST: Samourais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Samourai samourai = db.Samourais.Find(id);
            samourai.ArtsMartiaux.Clear();
            db.Samourais.Remove(samourai);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("DetacherArme")]
        public ActionResult DetacherArme(int? id)
        {
            Samourai samourai = db.Samourais.Find(id);
            samourai.Arme = null;
            db.SaveChanges();
            return RedirectToAction("Details",new { id = id });
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
}
