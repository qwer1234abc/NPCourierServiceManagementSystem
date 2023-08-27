$(document).ready(function () {
	// Get the current page URL
	var url = window.location.href;

	// Add 'active' class to the navigation link that matches the current URL
	$('.navbar-nav a').filter(function () {
		return this.href == url;
	}).addClass('active');
});