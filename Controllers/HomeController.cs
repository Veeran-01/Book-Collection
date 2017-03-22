using ClassicBookCollection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace ClassicBookCollection.Controllers
{
    public class HomeController : Controller
    {
        //create an instance of a database connection object to allow a connection to the database
        private BookDBEntities2 db = new BookDBEntities2();

        // GET: Home- main page
        public ActionResult Index(string bookGenre, string bookTimePeriod,  string searchString) /*, string searchAuthor*/
        {
            //genre search
            var GenreList = new List<string>();
            //gets genres from db and sort them
            var GenreQuery = from g in db.BookTable_1
                             orderby g.Genre
                             select g.Genre;

            //adds unique genres to the list
            GenreList.AddRange(GenreQuery.Distinct());

            //use the genre list to create a new select list for the 
            //dropdown menu in the index view and pass it to the 
            //view using the viewBag
            ViewBag.bookGenre = new SelectList(GenreList);



            //Timeperiod search
            var TimeList = new List<string>();
            //gets time period from db and sort them
            var TimeQuery = from g in db.BookTable_1
                            orderby g.Time_Period
                             select g.Time_Period;

            //adds unique time period to the list
            TimeList.AddRange(TimeQuery.Distinct());

            //use the timeperiod list to create a new select list for the 
            //dropdown menu in the index view and pass it to the 
            //view using the viewBag
            ViewBag.bookTimePeriod = new SelectList(TimeList);
            
            //LINQ Query to select all records in the database
            var books = from bk in db.BookTable_1 select bk;

            //title search
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString)); //.Contains- so they don't have to type the whole books name
                //books = books.Where(s => s.Author.Contains(searchString));

                
            }
            ////Author search
            //if (!String.IsNullOrEmpty(searchAuthor))
            //{
            //    books = books.Where(s => s.Author.Contains(searchAuthor)); //.Contains- so they don't have to type the whole books name
            //}

            //last bit of genre search
            if (!String.IsNullOrEmpty(bookGenre))
            {
                books = books.Where(x => x.Genre == bookGenre);
            }
            //pass books data to the view
           
            //timep period search
            if (!String.IsNullOrEmpty(bookTimePeriod))
            {
                books = books.Where(x => x.Time_Period == bookTimePeriod);
            }
            //pass books data to the view
            return View(books);
        }


        //to add a new book
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(BookTable_1 book)
        {
            //if user doesn't enter anything in Image URL section will display image of book in tiny url
            if (book.ImageURL == null)
            {
                book.ImageURL = "~/Content/images/bookImage.jpg";
                
            }
            if (ModelState.IsValid)
            {
                //Saves new book to the db
                db.BookTable_1.Add(book);
                db.SaveChanges();
                //Return to the index Action Method after adding book (i.e. will go back to index page after book added)
                return RedirectToAction("Index");
            }
           
            return View(book); //return to view page if not valid
        }


        //to view details of movie
        public ActionResult Details(int? id)
        {
            if (id == null)     //if don't have id to search details i.e. not in URL- takes user to error pages
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BookTable_1 book = db.BookTable_1.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        //to edit book details
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTable_1 book = db.BookTable_1.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);

        }

        [HttpPost]
        public ActionResult Edit(BookTable_1 book)
        {
            if (ModelState.IsValid)
            {
                //Write the edited record to the db
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();

                //go back to the index afer editing
                return RedirectToAction("Index");

            }
            return View(book);

        }


        //to delete a book
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find book to delete
            BookTable_1 book = db.BookTable_1.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);

        }

        //alias to avoid clash of method signatures
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        { 
            //find the book to be deleted
            BookTable_1 book = db.BookTable_1.Find(id);
            //delete the book to be deleted
            db.BookTable_1.Remove(book);
            db.SaveChanges();
            //go back to the index action method
            return RedirectToAction("Index");

        }

    }
}