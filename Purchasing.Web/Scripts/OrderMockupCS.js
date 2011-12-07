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
      // Do we need to build a dynamic form?
      var match = /items\[([0-9]+)\].quantity/i.exec(el)
      if(match != null) {
        // Set a line-item. Form only has three by default. Add one if we see items[3] or greater.
        // Note that one line-item is a group of a few elements that will trigger the callback.
        // Only respond to items[?].quantity (arbitrary. we could respond to any single one in the group)
        if(parseInt(match[1]) > 2) {
          // found a line item that requires we expand the DOM
          
          // FIXME: This isn't correct. If we see a items[54], we need to ensure there are 54, not simply add
          // one that will create items[3]. 
          $("#add-line-item").trigger('click');
          
          
        }
      }
      
      $(el).val(val); // presuming any dynamic bits were created, set the element
    }
  });
}

// Clear out a saved order form
function clearStoredOrderForm() {
  localStorage.removeItem("orderform");
}

// TODO: Save automatically every five seconds
