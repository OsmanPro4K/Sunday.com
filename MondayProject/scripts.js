// JavaScript to toggle sidebar
let sidebarBtn = document.getElementById("sidebarCollapse");
let outerSidebarBtn = $('#sidebarCollapseOuter')

$(document).ready(() => {
    outerSidebarBtn.hide();
})

sidebarBtn.addEventListener("click", () => {
    const sidebar = document.getElementById("sidebar");
    const mainContent = document.getElementById("mainContent");
    sidebar.classList.toggle('collapsed');
    if (sidebar.classList.contains('collapsed')) {
        //mainContent.style.marginLeft = "50px";
        mainContent.style.width = "calc(100% - 50px)";
        outerSidebarBtn.show();
        sidebar.style.paddingTop = '0';
    } else {
        //mainContent.style.marginLeft = "20%";
        mainContent.style.width = "80%";
        sidebarBtn.innerHTML = "<";
        outerSidebarBtn.hide();
        sidebar.style.paddingTop = '20px';
    }
});

outerSidebarBtn.on("click", () => {
    const sidebar = document.getElementById("sidebar");
    const mainContent = document.getElementById("mainContent");
    sidebar.classList.toggle('collapsed');
    if (sidebar.classList.contains('collapsed')) {
        //mainContent.style.marginLeft = "50px";
        mainContent.style.width = "calc(100% - 50px)";
        outerSidebarBtn.show();
    } else {
        //mainContent.style.marginLeft = "20%";
        mainContent.style.width = "80%";
        outerSidebarBtn.innerHTML = ">";
        outerSidebarBtn.hide();
    }
});

// Search Bar Focus
document.addEventListener('DOMContentLoaded', () => {
    // Search Bar Focus
    let searchBar = document.querySelector(".search-bar");
    let searchBarDiv = document.querySelector(".search-bar-div");

    searchBar.addEventListener("focus", () => {
        searchBarDiv.classList.add("focused");
    });

    searchBar.addEventListener("blur", () => {
        searchBarDiv.classList.remove("focused");
    });
});

// Add Board
let addBoardMenu = document.querySelector('.add-board-menu');
let addBoardBtn = document.querySelector('.add-board-btn');

addBoardBtn.addEventListener('click', (event) => {
    event.preventDefault();
    addBoardMenu.removeAttribute("hidden");
});

// Board Menu
let boardMenuCancelBtn = $('.add-board-menu-cancel-btn');
let boardMenu = $('.add-board-menu');

boardMenuCancelBtn.on('click', () => {
    boardMenu.attr('hidden', 'hidden');
});

// Display Options Menu
function showOptionsMenu(button) {
    const optionsMenu = $('.options-menu');
    const buttonOffset = $(button).offset();
    const buttonWidth = $(button).outerWidth();

    optionsMenu.css({
        top: buttonOffset.top,
        left: buttonOffset.left - optionsMenu.outerWidth() + 65 // Adjust 10px for some spacing
    });

    // Show the options menu
    optionsMenu.show();
}

$('.options-btn').on('click', function () {
    // Get the id of the clicked button
    const buttonId = $(this).attr('id');
    console.log('Button ID:', buttonId);

    showOptionsMenu(this);
});

$(document).on('click', function (event) {
    if (!$(event.target).closest('.options-btn, .options-menu').length) {
        $('.options-menu').hide();
    }
});

// Display Status Menu
function showStatusMenu(button) {
    const statusMenu = $('.status-menu');
    const buttonOffset = $(button).offset();
    const buttonHeight = $(button).outerHeight();

    statusMenu.css({
        top: buttonOffset.top - statusMenu.outerHeight() - 10, // Adjust 10px for some spacing
        left: buttonOffset.left
    });

    statusMenu.show();
}

$(document).on('click', '.status-btn', function () {
    const buttonId = $(this).attr('id');
    console.log('Button ID:', buttonId);

    showStatusMenu(this);
});

function hideStatusMenu() {
    let statusMenu = $('.status-menu');
    statusMenu.hide();
}

function showCalendarMenu(button) {
    let calendarMenu = $('.calendar-menu');
    calendarMenu.show();

    console.log("showCalendarMenu was executed")
}
$(document).on('click', '.date-btn', function () {
    const buttonId = $(this).attr('id');
    console.log('Button ID:', buttonId);

    showCalendarMenu(this);
});

// Invite New Member Menu
let searchPeopleBar = $('.search-people-bar');
let searchPeopleDiv = $('.search-people-div');
let cancelSearchTextBtn = $('.search-people-bar-btn')

searchPeopleBar.on('focus', () => {
    searchPeopleDiv.css('border-color', "#0065C5");
})

searchPeopleBar.on('blur', () => {
    searchPeopleDiv.css('border-color', "#c3c6d4");
})

searchPeopleBar.on('input', () => {
    if (searchPeopleBar.val().length > 0) {
        cancelSearchTextBtn.show();
    } else {
        cancelSearchTextBtn.hide();
    }
})

cancelSearchTextBtn.on('click', () => {
    searchPeopleBar.val('');
})

$(document).on('click', function (event) {
    if (!$(event.target).closest('.people-btn, .people-menu').length) {
        $('.people-menu').attr('hidden', 'hidden');
    }
    if (!$(event.target).closest('.add-new-member-cancel-btn, .add-new-member').length) {
        $('.add-new-member').attr('hidden', 'hidden');
    }
});

// Table
/*
$('.task-table th').on('click', function () {
    var thValue = $(this).text();

    if (thValue.trim() !== "Task") {
        $(this).html('<input type="text" class="editable-th" value="' + thValue + '">');
        $('.editable-th').focus();
    }

    console.log("th was clicked")
});
*/
$(document).on('blur', '.editable-th', function () {

    var newValue = $(this).val();

    $(this).parent().text(newValue);
});

// Discussions Menu

// // Change post to textbox for editing
function changeToTextbox(button, labelID, textboxID) {

    let label = document.getElementById(labelID);
    let textbox = document.getElementById(textboxID);
    let boardIcon = document.querySelector('.board-icon');

    textbox.value = label.innerText;
    label.style.display = 'none';
    boardIcon.style.display = 'none';
    textbox.style.display = 'inline';

    console.log(button.innerHTML);
}


// // Hovering on a reply displays the options
$(document).ready(function () {
    $('.reply-div').each(function () {
        let replyDiv = $(this);
        let replyOptionsMenu = replyDiv.find('.reply-options-menu');

        replyDiv.on('mouseover', function () {
            replyOptionsMenu.show();
        });

        replyDiv.on('mouseleave', function () {
            replyOptionsMenu.hide();
        });
    });
});

// Show the options menu when hovering over the board button
$('.board-btn-div').on('mouseover', function () {
    $(this).find('.board-options-dropdown').show();
});
$('.board-btn-div').on('mouseleave', function () {
    $(this).find('.board-options-dropdown').hide();
});


// Resize header textbox to fit content
function resizeInput($input) {
    $input.css('width', 'auto'); // Reset the width
    $input.css('width', ($input[0].scrollWidth + 10) + 'px'); // Adjust width based on content
}

// Initial resize for the default value
$(document).ready(function () {
    const $input = $('.header-textbox');
    resizeInput($input);
    $input.on('input', function () {
        resizeInput($(this));
    });
});

// Toggle People's Menu
// Toggle People's Menu
function togglePeoplesMenu(button) {
    const peopleMenu = $('.people-menu');
    const buttonOffset = $(button).offset();
    const buttonWidth = $(button).outerWidth();

    if (peopleMenu.is(':visible')) {
        peopleMenu.hide();
    } else {
        peopleMenu.css({
            top: buttonOffset.top + buttonWidth + 10, // Adjust for spacing if needed
            left: buttonOffset.left - peopleMenu.outerWidth() - 10 // Adjust for spacing if needed
        });
        peopleMenu.show();
    }
}

$('.people-btn').on('click', function (event) {
    event.stopPropagation(); // Prevent the click from bubbling up
    togglePeoplesMenu(this);
});

$(document).on('click', function (event) {
    if (!$(event.target).closest('.people-btn, .people-menu').length) {
        $('.people-menu').hide();
    }
});

// show discussions menu
function toggleDiscussionsMenu(button) {
    const discussionMenu = $('.task-discussion-menu');

    discussionMenu.show();
}

$('.comments-btn').on('click', function (event) {
    event.stopPropagation(); // Prevent the click from bubbling up
    toggleDiscussionsMenu(this);
});

// options btn in nav tabs
$(document).ready(function () {
    $('.nav-link-label').addClass('move-left');
});

$('.nav-item').on('mouseover', function () {
    let label = $(this).find('.nav-link-label'); // Scope to the current .nav-item
    label.removeClass('move-left');

    let navOptionsBtn = $(this).find('.nav-options-btn'); // Adjust selector as needed
    navOptionsBtn.css({ 'opacity': '1' });
});

$('.nav-item').on('mouseleave', function () {
    let label = $(this).find('.nav-link-label'); // Scope to the current .nav-item
    label.addClass('move-left');

    let navOptionsBtn = $(this).find('.nav-options-btn'); // Adjust selector as needed
    navOptionsBtn.css({ 'opacity': '0' });
});

