use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Contact {
    pub name: String,
    pub phone: String,
    pub email: String,
}

#[derive(Serialize, Deserialize)]
pub struct ContactBook {
    contacts: Vec<Contact>,
}

impl ContactBook {
    pub fn new() -> Self {
        Self { contacts: Vec::new() }
    }

    pub fn add_contact(&mut self, name: String, phone: String, email: String) {
        self.contacts.push(Contact { name, phone, email });
        println!("Contact added successfully!");
    }

    pub fn delete_contact(&mut self, query: &str) {
        let initial_len = self.contacts.len();
        self.contacts.retain(|c| c.name != query && c.email != query);
        
        if self.contacts.len() < initial_len {
            println!("Contact deleted successfully!");
        } else {
            println!("No matching contact found.");
        }
    }

    pub fn search(&self, query: &str) {
        let results: Vec<&Contact> = self.contacts.iter()
            .filter(|c| c.name.contains(query) || c.email.contains(query))
            .collect();
        
        if results.is_empty() {
            println!("No contacts found.");
        } else {
            for contact in results {
                println!("Found: {} - {} - {}", contact.name, contact.phone, contact.email);
            }
        }
    }

    pub fn list_all(&self) {
        if self.contacts.is_empty() {
            println!("No contacts available.");
        } else {
            println!("\nAll Contacts:");
            for contact in &self.contacts {
                println!("{} - {} - {}", contact.name, contact.phone, contact.email);
            }
        }
    }

    // Save to file
    pub fn save_to_file(&self, file_path: &str) {
        let file = std::fs::File::create(file_path).expect("Unable to create file");
        serde_json::to_writer_pretty(file, &self).expect("Unable to write data to file");
        println!("Contacts saved to file!");
    }

    // Load from file
    pub fn load_from_file(file_path: &str) -> Self {
        if let Ok(file) = std::fs::File::open(file_path) {
            serde_json::from_reader(file).unwrap_or_else(|_| ContactBook::new())
        } else {
            ContactBook::new() // Return an empty ContactBook if the file doesn't exist
        }
    }

    pub fn is_valid_email(email: &str) -> bool {
        let parts: Vec<&str> = email.trim().split('@').collect();
        parts.len() == 2 && !parts[0].is_empty() && !parts[1].is_empty()
    }
    pub fn is_valid_phone(phone: &str) -> bool {
        phone.trim().chars().all(|c| c.is_ascii_digit())
    }
}
