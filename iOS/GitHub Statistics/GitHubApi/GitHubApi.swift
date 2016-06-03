//
//  GitHubApi.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 02/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import Foundation
import Utils

public class GitHubApi {
    public init() {        
    }
    
    public func searchUsers(phrase: String, completitionHandler: ([(username: String, avatarUrl: String?)]) -> Void) {
        NetworkUtils.requestJson("https://api.github.com/search/users?q=\(phrase)") { json in
            if let items = json["items"] as? NSArray {
                let tuples = items.filter { $0["login"] != nil }.map { (username: $0["login"] as! String, avatarUrl: $0["avatar_url"] as? String) }
                completitionHandler(tuples)
            }
        }
    }
}
