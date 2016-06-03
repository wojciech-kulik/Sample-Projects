//
//  FirstViewController.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 01/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import UIKit
import GitHubApi
import Utils

class SearchViewController: UIViewController, UITableViewDataSource, UITableViewDelegate, UISearchBarDelegate {

    @IBOutlet weak var searchBar: UISearchBar!
    @IBOutlet weak var recentlySearchedTableView: UITableView!
    
    let loadingOverlayController = UILoadingOverlayController(nibName: "UILoadingOverlay", bundle: nil)
    let gitHubApi = GitHubApi()
    
    var lastSearchPhrase: String?
    var imageCache = [String:NSData]()
    var recentlySearched = [String]()
    var users = [(username: String, avatarUrl: String?)]()
    lazy var searchResultsTableView: UITableView = self.searchDisplayController!.searchResultsTableView
    var searchActive : Bool {
        get { return searchBar.text != nil && searchBar.text != "" }
    }
    
    override func viewDidLoad() {
        self.searchResultsTableView.separatorStyle = .None
        recentlySearched.append("test1")
        recentlySearched.append("test2")
        recentlySearched.append("test3")
    }
    
    // MARK: - API Operations
    
    func downloadAvatarFor(username: String, avatarUrl: String, tableView: UITableView, cell: UITableViewCell, indexPath: NSIndexPath) {
        if let url = NSURL(string: avatarUrl) {
            if let data = self.imageCache[username] {
                cell.imageView!.image = UIImage(data: data)
            } else {
                print("downloading avatar for \(username)")
                NetworkUtils.downloadImageForCell(url, tableView: tableView, index: indexPath) { imageData in
                    dispatch_async(dispatch_get_main_queue()) {
                        self.imageCache[username] = imageData
                    }
                }
            }
        }
    }
    
    func getUsers(searchPhrase: String) {
        print("start search \(searchPhrase)")
        
        self.gitHubApi.searchUsers(searchPhrase) { users in
            dispatch_async(dispatch_get_main_queue()) {
                if self.lastSearchPhrase != searchPhrase {
                    return
                }
                
                self.addToRecentlySearched(searchPhrase)
                
                print("results received for \(searchPhrase)")
                self.users = users
                self.searchResultsTableView.reloadData()
                self.hideProgress()
                self.searchBar.showsCancelButton = true
            }
        }
    }
    
    // MARK: - SearchBar
    
    func searchBarTextDidBeginEditing(searchBar: UISearchBar) {
        setNoResultsText("")
        self.searchBar.showsCancelButton = true
    }
    
    func searchBarTextDidEndEditing(searchBar: UISearchBar) {
        self.searchBar.showsCancelButton = false
        if searchBar.text == "" {
            self.imageCache.removeAll()
            self.lastSearchPhrase = nil
        }
    }

    func searchBarCancelButtonClicked(searchBar: UISearchBar) {
        hideProgress()
        self.users.removeAll()
        self.searchBar.text = nil
        self.imageCache.removeAll()
        self.lastSearchPhrase = nil
        self.searchBar.showsCancelButton = false
    }
    
    func searchBar(searchBar: UISearchBar, textDidChange searchText: String) {
        setNoResultsText("")
        if searchText == "" {
            self.users.removeAll()
        }
    }
    
    func searchBarSearchButtonClicked(searchBar: UISearchBar) {
        let lowerSearchText = searchBar.text!.lowercaseString
        
        if self.searchActive && self.lastSearchPhrase?.lowercaseString != lowerSearchText {
            self.lastSearchPhrase = lowerSearchText
            showProgress()
            
            let triggerTime = (Int64(NSEC_PER_SEC) * 1)
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, triggerTime), dispatch_get_main_queue()) {
                if self.lastSearchPhrase != lowerSearchText {
                    return
                }
                
                self.getUsers(lowerSearchText)
            }
        } else {
            self.lastSearchPhrase = nil
            self.users.removeAll()
            hideProgress()
        }
    }
    
    func addToRecentlySearched(lowerSearchText: String) {
        if (!self.recentlySearched.contains(lowerSearchText)) {
            self.recentlySearched.insert(lowerSearchText, atIndex: 0)
            if (self.recentlySearched.count > 10) {
                self.recentlySearched.removeLast()
            }
            self.recentlySearchedTableView.reloadData()
        }
    }
    
    // MARK: - TableView
    
    func numberOfSectionsInTableView(tableView: UITableView) -> Int {
        return 1
    }
    
    func tableView(tableView: UITableView, viewForFooterInSection section: Int) -> UIView? {
        return UIView()
    }
    
    func tableView(tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        return tableView == self.recentlySearchedTableView ? "Recently searched" : nil
    }
    
    func tableView(tableView: UITableView, heightForHeaderInSection section: Int) -> CGFloat {
        return tableView != self.recentlySearchedTableView ? 5 : 20
    }
    
    func tableView(tableView: UITableView, viewForHeaderInSection section: Int) -> UIView? {
        if tableView != self.recentlySearchedTableView {
            return nil
        }
        
        let headerHeight = self.tableView(tableView, heightForHeaderInSection: section)
        let view = UIView(frame: CGRect(x: 0, y: 0, width: tableView.frame.width, height: headerHeight))
        let label = UILabel(frame: CGRect(x: 15, y: 0, width: tableView.frame.width, height: headerHeight))
        
        label.text = self.tableView(tableView, titleForHeaderInSection: section)
        label.font = label.font.fontWithSize(12)
        label.textColor = UIColor.whiteColor()
        
        view.backgroundColor = UIColor(red: 37.0 / 255.0, green: 37.0 / 255.0, blue: 37.0 / 255.0, alpha: 1.0)
        view.addSubview(label)
        return view
    }
    
    func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return tableView == self.searchResultsTableView ? self.users.count : self.recentlySearched.count;
    }
    
    func tableView(tableView: UITableView, cellForRowAtIndexPath indexPath: NSIndexPath) -> UITableViewCell {
        var cell: UITableViewCell!
        
        if tableView == self.searchResultsTableView {
            cell = tableView.dequeueReusableCellWithIdentifier("searchCell") ??
                SearchResultCell(style: .Default, reuseIdentifier: "searchCell")
            
            let username = self.users[indexPath.row].username
            cell.imageView!.image = UIImage(named: "no_avatar")
            cell.accessoryType = .DisclosureIndicator
            cell.textLabel?.text = username
            
            if let avatarUrl = self.users[indexPath.row].avatarUrl {
                downloadAvatarFor(username, avatarUrl: avatarUrl, tableView: tableView, cell: cell, indexPath: indexPath)
            }
        } else {
            cell = tableView.dequeueReusableCellWithIdentifier("cell", forIndexPath: indexPath)
            (cell as! RecentSearchCell).setupSelectionColor()
            cell.textLabel?.text = self.recentlySearched[indexPath.row]
        }
        
        return cell;
    }
    
    func tableView(tableView: UITableView, didSelectRowAtIndexPath indexPath: NSIndexPath) {
        if tableView == self.recentlySearchedTableView {
            self.searchDisplayController!.setActive(true, animated: true)
            self.searchBar.text = self.recentlySearched[indexPath.row]
            searchBarSearchButtonClicked(self.searchBar)
        } else {
            let cell = tableView.cellForRowAtIndexPath(indexPath)
            self.performSegueWithIdentifier("repositoryListNavigation", sender: cell)
        }
        tableView.deselectRowAtIndexPath(indexPath, animated: true)
    }
    
    override func prepareForSegue(segue: UIStoryboardSegue, sender: AnyObject?) {
        if segue.identifier == "repositoryListNavigation" {
            let destination = segue.destinationViewController as! RepositoryListViewController
            let cell = sender as! UITableViewCell
            let selectedRow = self.searchResultsTableView.indexPathForCell(cell)!.row
            destination.selectedUsername = self.users[selectedRow].username
        }
    }

    // MARK: - Progress Indicator
    
    private func showProgress() {
        setNoResultsText("")
        self.loadingOverlayController.show(self.searchResultsTableView, frame: self.searchResultsTableView.bounds)
        self.loadingOverlayController.textLabel.text = "Searching..."
    }
    
    private func hideProgress() {
        self.loadingOverlayController.hide(self.searchResultsTableView)
        setNoResultsText("No Results")
    }
    
    private func setNoResultsText(text: String) {
        for subview in self.searchResultsTableView.subviews {
            if let label = subview as? UILabel {
                label.text = text
            }
        }
    }
}