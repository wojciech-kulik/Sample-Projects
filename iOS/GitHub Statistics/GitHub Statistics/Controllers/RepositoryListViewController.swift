//
//  RepositoryListViewController.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 02/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import UIKit
import GitHubApi

class RepositoryListViewController : UIViewController {
    
    var selectedUsername: String?
    
    override func viewDidLoad() {
        self.navigationItem.title = self.selectedUsername
    }
}