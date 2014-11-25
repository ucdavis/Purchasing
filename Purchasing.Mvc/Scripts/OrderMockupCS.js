// Serialize and unserialize the order form in parts, storing them in LocalStorage key/value store

// Save the current state of the order form
function cacheOrderForm() {
  // TODO: Check to see if there's an order form already and fail if so
  localStorage.setItem('orderform', $("form[name=orderform]").serialize());
  
  return true;
}

// Restore a saved state of the order form
function restoreOrderForm() {
  // TODO: Give the user a notice that the file upload was not saved
  
  var data = localStorage.getItem('orderform');
  
  $("form[name=orderform]").unserializeForm(data, {
    'callback'  : function(el_name, val) {

      // Do we need to create any dynamic line item rows?
      var match = /items\[([0-9]+)\].quantity/i.exec(el_name) // look for array notation to indicate dynamic elements
      if(match != null) {
        // Set a line-item. Form only has three by default. Add one if we see items[3] or greater.
        // Note that one line-item is a group of a few elements that will trigger the callback.
        // Only respond to items[?].quantity (Looking for 'quantity' is arbitrary)
		    var items_index = parseInt(match[1]); // equals the 32 part of items[32].quantity
          
	  	  // Ensure we have enough line items (i.e. if we see items[54] but only have 34 rows, add 20 more)
	  	  var num_line_items = $("tbody#line-items-body tr.line-item-row").length;
		
		    while(items_index >= num_line_items) {  
      		purchasing.addLineItem();
			    items_index--;
		    }
      }
	  
	    // TODO: any other dynamic elements
	    
      var el = $(this).add("input,select,textarea").find("[name=\"" + el_name + "\"]");
      
      console.log("setting " + el_name + " to value " + val);
      $(el).val(val); // presuming any dynamic bits were created, set the element
    }
  });
  
  return true;
}

// Clear out a saved order form
function clearStoredOrderForm() {
  localStorage.removeItem("orderform");
}

// TODO: Save automatically every five seconds
