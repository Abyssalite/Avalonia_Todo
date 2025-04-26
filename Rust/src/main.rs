use std::sync::{Arc, Mutex};
use std::thread;
use std::io::{self, Write};
mod contact;
use contact::ContactBook;

fn shared_counter() {
    let store = Arc::new(Mutex::new(0)); // Shared store inside Arc<Mutex<T>>

    let mut handles = vec![];

    for _ in 0..5 {
        let store_clone = Arc::clone(&store);

        let handle = thread::spawn(move || {
            let mut num = store_clone.lock().unwrap();
            *num += 1;
        });

        handles.push(handle);
    }

    for handle in handles {
        handle.join().unwrap(); // Wait for all threads to finish
    }

    println!("Final count: {}", *store.lock().unwrap());
}

const CONTACT_FILE: &str = "contacts.json";

fn main() {
    let mut name = String::new();
    let mut phone = String::new();
    let mut email = String::new();
    shared_counter();
    let mut contact_book = ContactBook::load_from_file(CONTACT_FILE);
    
    loop {
        println!("\n1. Add Contact\n2. Delete Contact\n3. Search Contact\n4. List All Contacts\n5. Save and Exit\nChoose an option: ");
        
        let mut choice = String::new();
        io::stdin().read_line(&mut choice).expect("Failed to read input");
        
        match choice.trim() {
            "1" => {
                get_contact_details(&mut name, &mut phone, &mut email);
                contact_book.add_contact(name.trim().to_string(), phone.trim().to_string(), email.trim().to_string());
            }
            "2" => {
                println!("Enter name or email to delete: ");
                let mut query = String::new();
                io::stdin().read_line(&mut query).expect("Failed to read input");
                contact_book.delete_contact(query.trim()); 
            }
            "3" => {
                println!("Enter name or email to search: ");
                let mut query = String::new();
                io::stdin().read_line(&mut query).expect("Failed to read input");
                contact_book.search(query.trim());
            }
            "4" => {
                contact_book.list_all();
            }
            "5" => {
                contact_book.save_to_file(CONTACT_FILE);
                break;
            }
            _ => println!("Invalid choice, try again."),
        }
    }
}

fn get_contact_details(name: &mut String, phone: &mut String, email: &mut String,) {


    print!("Enter Name: ");
    io::stdout().flush().unwrap();
    io::stdin().read_line(name).expect("Failed to read input");
    
    loop {
        print!("Enter Phone: ");
        io::stdout().flush().unwrap();
        io::stdin().read_line(phone).expect("Failed to read input");

        if ContactBook::is_valid_phone(&phone) {
            break;
        } else {
            println!("Invalid phone format! Please enter only phone number");
            phone.clear();
        }
    }
    
    loop {
        print!("Enter Email: ");
        io::stdout().flush().unwrap();
        io::stdin().read_line(email).expect("Failed to read input");

        if ContactBook::is_valid_email(&email) {
            break;
        } else {
            println!("Invalid email format! Please enter a valid email containing '@'.");
            email.clear();
        }
    }

}