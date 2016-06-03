//
//  NetworkUtils.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 02/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import Foundation
import UIKit

public final class NetworkUtils {
    private init() { }
    
    public static func getDataFromUrl(url:NSURL, completion: ((data: NSData?, response: NSURLResponse?, error: NSError? ) -> Void)) {
        NSURLSession.sharedSession().dataTaskWithURL(url) { (data, response, error) in
            completion(data: data, response: response, error: error)
        }.resume()
    }
    
    public static func downloadImageForCell(url: NSURL, tableView: UITableView, index: NSIndexPath) {
        downloadImageForCell(url, tableView: tableView, index: index, completition: nil)
    }
    
    public static func downloadImageForCell(url: NSURL, tableView: UITableView, index: NSIndexPath, completition: ((image: NSData) -> Void)?) {
        getDataFromUrl(url) { (data, response, error)  in
            dispatch_async(dispatch_get_main_queue()) {
                guard let data = data where error == nil else { return }
                
                if let cell = tableView.cellForRowAtIndexPath(index) {
                    cell.imageView!.image = UIImage(data: data)
                    cell.setNeedsLayout()
                }
                
                if completition != nil {
                    completition!(image: data)
                }
            }
        }
    }
    
    public static func requestJson(url: String, completition: (json: NSDictionary) -> Void) {
        let nsUrl: NSURL = NSURL(string: url)!
        let request = NSMutableURLRequest(URL: nsUrl)
        request.HTTPMethod = "GET"
        request.setValue("application/json", forHTTPHeaderField: "Accept")
        
        NSURLSession.sharedSession().dataTaskWithRequest(request) { data, response, error in
            do {
                if let jsonResult = try NSJSONSerialization.JSONObjectWithData(data!, options: []) as? NSDictionary {
                    completition(json: jsonResult)
                }
            } catch let error as NSError {
                print(error.localizedDescription)
            }
        }.resume()
    }
}
