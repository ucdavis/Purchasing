// Serialize and unserialize the order form in parts, storing them in LocalStorage key/value store

// Save the current state of the order form
function cacheOrderForm() {
  // TODO: Check to see if there's an order form already and fail if so
  localStorage.setItem('orderform', $("form[name=orderform]").serialize());
  
  return true;
}

// Restore a saved state of the order form
function restoreOrderForm() {
  // Give the user a notice that the file upload was not saved
  
  // First, ensure we have the proper number of line items
  var data = localStorage.getItem('orderform');
  
  $("form[name=orderform]").unserializeForm(data, {
    'callback'  : function(el, val) {
      // TODO: Build the form dynamically here
      $(el).val(val); // just set the element for now
    }
  });
}

// Clear out a saved order form
function clearStoredOrderForm() {
  localStorage.removeItem("justification");
  localStorage.removeItem("vendor");
  localStorage.removeItem("shipping");
}

// TODO: UI/UX to prompt when leaving/loading page
