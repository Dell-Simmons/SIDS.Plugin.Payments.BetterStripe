// This is your test publishable API key.

//try {
//  alert("The first attempt to access Stripe: " + Stripe.version);
//} catch (e) {
//  alert("Stripe is not ready.");
//}
// 
var pKey;
var stripe;
var stripeScriptElement = document.querySelector("script[src^='https://js.stripe.com/v3']");
if (stripeScriptElement) {
  stripeScriptElement.addEventListener("load", () => {
    if (Stripe) {
     // alert("Stripe v." + Stripe.version + " is ready. ");
      stripe = Stripe(pKey);
     // alert("pKey in checkout.js is" + pKey);
    }
    else {
      alert("Failed loading Stripe.js");
    }
  });
}


// The items the customer wants to buy
const items = [{ id: "xl-tshirt" }];

let elements;

initialize();
checkStatus();

document
  .querySelector("#payment-form")
  .addEventListener("submit", handleSubmit);

let emailAddress = '';


//var options = new PaymentIntentCreateOptions
//{
//    Amount = 2000,
//        Currency = "usd",
//        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
//    {
//        Enabled = true,
//  },
//};
//var service = new PaymentIntentService();
//service.Create(options);
// bottom
// Fetches a payment intent and captures the client secret

async function initialize() {
  const response = await fetch("/create-payment-intent", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ items }),
  });
  const { clientSecret } = await response.json();

  const appearance = {
    theme: 'stripe',
  };
  elements = stripe.elements({ appearance, clientSecret });


  const paymentElementOptions = {
    layout: "tabs",
  };

  const paymentElement = elements.create("payment", paymentElementOptions);
  paymentElement.mount("#payment-element");
  //alert("disable the continue button????")
  //$('.payment-info-next-step-button').attr('onclick', null);
}
$('input.payment-info-next-step-button').on('click', function (data) {
  event.preventDefault();
/*$('.payment-info-next-step-button').on('click', function (data) {*/
  alert("button clicked");
/*  $('#braintree-errors').html('');*/
  alert("button clicked 3");
  //if (!submitForm) {
  //  if (!stepBack) {
  //    components.hostedFields.tokenize(function (err, payload) {
  //      if (err) {
  //        console.log('tokenization error:', err);

  //        var currentErrorvalue = $('#@Html.IdFor(model => model.Errors)').val();
  //        $('#@Html.IdFor(model => model.Errors)').val(currentErrorvalue + '|' + err.message);

  //        $('#braintree-errors').html(err.message);
  //        return;
  //      }

  //      verifyCard(payload);
  //    });
  //  }
  //  return false;
  //}
  //else if (onePageCheckout) {
  //  submitForm = false;
  alert("Calling handleSubmit");
  handleSubmit();
  alert("returned from hanndleSubmit");
  alert("calling Payment.Save");
    PaymentInfo.save();
  //}
});

async function handleSubmit(e) {
  e.preventDefault();
  setLoading(true);
  alert("in handleSubmit above stripe.confirmPayment");
  const { error } = await stripe.confirmPayment({
    elements,
    confirmParams: {
      // Make sure to change this to your payment completion page
      return_url: '@(storeLocation)checkout/OpcSavePaymentInfo/',
      receipt_email: emailAddress,
    },
  });

  // This point will only be reached if there is an immediate error when
  // confirming the payment. Otherwise, your customer will be redirected to
  // your `return_url`. For some payment methods like iDEAL, your customer will
  // be redirected to an intermediate site first to authorize the payment, then
  // redirected to the `return_url`.
  if (error.type === "card_error" || error.type === "validation_error") {
    showMessage(error.message);
  } else {
    showMessage("An unexpected error occurred.");
  }

  setLoading(false);
}

// Fetches the payment intent status after payment submission
async function checkStatus() {
  const clientSecret = new URLSearchParams(window.location.search).get(
    "payment_intent_client_secret"
  );

  if (!clientSecret) {
    return;
  }

  const { paymentIntent } = await stripe.retrievePaymentIntent(clientSecret);

  switch (paymentIntent.status) {
    case "succeeded":
      showMessage("Payment succeeded!");
      break;
    case "processing":
      showMessage("Your payment is processing.");
      break;
    case "requires_payment_method":
      showMessage("Your payment was not successful, please try again.");
      break;
    default:
      showMessage("Something went wrong.");
      break;
  }
}

// ------- UI helpers -------

function showMessage(messageText) {
  const messageContainer = document.querySelector("#payment-message");

  messageContainer.classList.remove("hidden");
  messageContainer.textContent = messageText;

  setTimeout(function () {
    messageContainer.classList.add("hidden");
    messageContainer.textContent = "";
  }, 4000);
}

// Show a spinner on payment submission
function setLoading(isLoading) {
  if (isLoading) {
    // Disable the button and show a spinner
    document.querySelector("#submit").disabled = true;
    document.querySelector("#spinner").classList.remove("hidden");
    document.querySelector("#button-text").classList.add("hidden");
  } else {
    document.querySelector("#submit").disabled = false;
    document.querySelector("#spinner").classList.add("hidden");
    document.querySelector("#button-text").classList.remove("hidden");
  }
}