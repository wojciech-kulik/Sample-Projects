//
//  RecentSearchCell.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 03/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import Foundation
import UIKit

class RecentSearchCell: UITableViewCell {
    override init(style: UITableViewCellStyle, reuseIdentifier: String?) {
        super.init(style: style, reuseIdentifier: reuseIdentifier)
        
        self.imageView!.image = UIImage(named: "search")
        self.accessoryType = .DisclosureIndicator
        setupSelectionColor()
    }
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
    }
    
    override func layoutSubviews() {
        super.layoutSubviews()
        
        imageView!.frame.origin.x = 5
        let imageRight = imageView!.frame.origin.x + imageView!.frame.width
        textLabel?.frame = CGRectMake(imageRight + 10, 0, textLabel!.frame.width, textLabel!.frame.height)
    }
    
    func setupSelectionColor() {
        self.selectedBackgroundView = UIView()
        self.selectedBackgroundView!.backgroundColor =
            UIColor(red: 249.0 / 255.0, green: 206.0 / 255.0, blue: 48.0 / 255.0, alpha: 1)
    }
}